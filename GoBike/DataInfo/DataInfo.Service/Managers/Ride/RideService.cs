using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Ride;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Ride.Content;
using DataInfo.Core.Models.Dto.Ride.Request;
using DataInfo.Core.Models.Dto.Ride.Response;
using DataInfo.Core.Models.Dto.Ride.View;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Repository.Interfaces.Interactive;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Repository.Interfaces.Ride;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Ride;
using DataInfo.Service.Interfaces.Server;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Ride
{
    /// <summary>
    /// 騎乘服務
    /// </summary>
    public class RideService : IRideService
    {
        /// <summary>
        /// interactiveRepository
        /// </summary>
        private readonly IInteractiveRepository interactiveRepository;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RideService");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        private readonly IMemberRepository memberRepository;

        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// rideRepository
        /// </summary>
        private readonly IRideRepository rideRepository;

        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// uploadService
        /// </summary>
        private readonly IUploadService uploadService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="uploadService">uploadService</param>
        /// <param name="serverService">serverService</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="interactiveRepository">interactiveRepository</param>
        /// <param name="rideRepository">rideRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public RideService(IMapper mapper, IUploadService uploadService, IServerService serverService, IMemberRepository memberRepository, IInteractiveRepository interactiveRepository, IRideRepository rideRepository, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.serverService = serverService;
            this.memberRepository = memberRepository;
            this.interactiveRepository = interactiveRepository;
            this.rideRepository = rideRepository;
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 轉換為組隊騎乘會員可視資料
        /// </summary>
        /// <param name="memberDao">memberDao</param>
        /// <param name="rideGroupMemberDaoMap">rideGroupMemberDaoMap</param>
        /// <returns>RideGroupMemberView</returns>
        private RideGroupMemberView TransformRideGroupMemberView(MemberDao memberDao, Dictionary<string, RideGroupMemberDao> rideGroupMemberDaoMap)
        {
            string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.GroupMember}_{memberDao.MemberID}";
            if (memberDao == null || rideGroupMemberDaoMap == null || !rideGroupMemberDaoMap.TryGetValue(cacheKey, out RideGroupMemberDao rideGroupMemberDao))
            {
                return null;
            }

            RideGroupMemberView view = new RideGroupMemberView()
            {
                Avatar = memberDao.Avatar,
                MemberID = memberDao.MemberID,
                Nickname = memberDao.Nickname,
                CoordinateX = rideGroupMemberDao.CoordinateX,
                CoordinateY = rideGroupMemberDao.CoordinateY
            };

            return view;
        }

        /// <summary>
        /// 新增騎乘資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> AddRideData(AddRideDataContent content, string memberID)
        {
            try
            {
                #region 檢視騎乘資料

                if (content.ShareContent == null)
                {
                    content.ShareContent = new string[][] { };
                }

                if (string.IsNullOrEmpty(content.Title))
                {
                    content.Title = $"{DateTime.UtcNow:yyyy/MM/dd}";
                }

                #endregion 檢視騎乘資料

                #region 上傳圖片

                List<string> imgBase64s = content.ShareContent.Select(data => data.ElementAt(1)).ToList();
                imgBase64s.Add(content.Photo);
                IEnumerable<string> imgUris = await this.uploadService.UploadRideImages(imgBase64s, true).ConfigureAwait(false);
                if (imgUris == null || !imgUris.Any())
                {
                    this.logger.LogWarn("新增騎乘資料失敗，上傳圖片失敗", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status502BadGateway,
                        ResultMessage = ResponseErrorMessageType.UploadPhotoFail.ToString()
                    };
                }

                content.ShareContent = content.ShareContent.Select((data, index) =>
                {
                    string imgUrl = imgUris.ElementAt(index);
                    if (string.IsNullOrEmpty(imgUrl))
                    {
                        this.logger.LogWarn("新增騎乘資料失敗，騎乘分享內容圖片轉換失敗", $"MemberID: {memberID} Data: {JsonConvert.SerializeObject(data)}", null);
                    }

                    return new List<string>()
                    {
                        data.ElementAt(0),
                        imgUrl
                    };
                });
                content.Photo = imgUris.LastOrDefault();

                #endregion 上傳圖片

                #region 發送【建立騎乘紀錄】指令至後端

                AddRideInfoRequest request = this.mapper.Map<AddRideInfoRequest>(content);
                request.MemberID = memberID;
                CommandData<AddRideInfoResponse> response = await this.serverService.DoAction<AddRideInfoResponse>((int)RideCommandIDType.CreateRideRecord, CommandType.Ride, request).ConfigureAwait(false);
                this.logger.LogInfo("新增騎乘資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)CreateRideRecordResultType.Success:
                        //string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.RideRecord}";
                        //this.redisRepository.DeleteCache(AppSettingHelper.Appsetting.Redis.RideDB, cacheKey);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)CreateRideRecordResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【建立騎乘紀錄】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("新增騎乘資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得好友週里程排名
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetFriendWeekRank(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.HomeInfo}";
                //IEnumerable<RideFriendWeekRankView> rideFriendWeekRankViews = await this.redisRepository.GetCache<IEnumerable<RideFriendWeekRankView>>(AppSettingHelper.Appsetting.Redis.RideDB, cacheKey).ConfigureAwait(false);
                IEnumerable<RideFriendWeekRankView> rideFriendWeekRankViews = null;
                if (rideFriendWeekRankViews == null)
                {
                    IEnumerable<string> friendIDList = await this.interactiveRepository.GetFriendList(memberID).ConfigureAwait(false);
                    Task<IEnumerable<RideDistanceDao>> rideDistanceDaosTask = this.rideRepository.GetWeekDistance(friendIDList);
                    Task<IEnumerable<MemberDao>> friendDaosTask = this.memberRepository.Get(friendIDList, null);

                    Dictionary<string, RideDistanceDao> rideDistanceMap = (await rideDistanceDaosTask.ConfigureAwait(false)).ToDictionary(data => data.MemberID);
                    IEnumerable<MemberDao> friendDaos = await friendDaosTask.ConfigureAwait(false);
                    rideFriendWeekRankViews = friendDaos.Select(data =>
                    {
                        rideDistanceMap.TryGetValue(data.MemberID, out RideDistanceDao rideDistanceDao);
                        RideFriendWeekRankView rideFriendWeekRankView = new RideFriendWeekRankView()
                        {
                            Avatar = data.Avatar,
                            Nickname = data.Nickname,
                            MemberID = data.MemberID
                        };
                        if (rideDistanceDao != null)
                        {
                            rideFriendWeekRankView.WeekDistance = rideDistanceDao.WeekDistance;
                        }

                        return rideFriendWeekRankView;
                    });

                    rideFriendWeekRankViews = rideFriendWeekRankViews.OrderByDescending(data => data.WeekDistance);
                    //this.redisRepository.SetCache(AppSettingHelper.Appsetting.Redis.RideDB, cacheKey, JsonConvert.SerializeObject(rideFriendWeekRankViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = rideFriendWeekRankViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得好友週里程排名發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得騎乘明細記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="rideID">rideID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetRideDetailRecord(string memberID, string rideID)
        {
            try
            {
                RideDao rideDao = await this.rideRepository.Get(memberID, rideID).ConfigureAwait(false);
                if (rideDao == null)
                {
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.GetFail.ToString()
                    };
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = this.mapper.Map<RideDetailRecordView>(rideDao)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得騎乘明細記錄發生錯誤", $"MemberID: {memberID} RideID: {rideID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得組隊隊員列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetRideGroupMemberList(string memberID)
        {
            try
            {
                List<RideGroupMemberView> rideGroupMemberViews = new List<RideGroupMemberView>();
                int redisDB = AppSettingHelper.Appsetting.Redis.ServerDB;
                string groupMemberCacheKey = AppSettingHelper.Appsetting.Redis.Flag.GroupMember;
                string cacheKey = $"{groupMemberCacheKey}_{memberID}";
                RideGroupMemberDao rideGroupMemberDao = await this.redisRepository.GetCache<RideGroupMemberDao>(redisDB, cacheKey).ConfigureAwait(false);
                if (rideGroupMemberDao != null)
                {
                    RideGroupDao rideGroupDao = await this.redisRepository.GetCache<RideGroupDao>(redisDB, rideGroupMemberDao.RideGroupKey).ConfigureAwait(false);
                    if (rideGroupDao != null)
                    {
                        IEnumerable<MemberDao> memberDaos = await this.memberRepository.Get(rideGroupDao.MemberList, null).ConfigureAwait(false);
                        Dictionary<string, RideGroupMemberDao> rideGroupMemberDaoMap = await this.redisRepository.GetCache<RideGroupMemberDao>(redisDB, rideGroupDao.MemberList.Select(id => $"{groupMemberCacheKey}_{id}")).ConfigureAwait(false);
                        rideGroupMemberViews = memberDaos.Select(memberDao => this.TransformRideGroupMemberView(memberDao, rideGroupMemberDaoMap)).Where(view => view != null).ToList();
                    }
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = rideGroupMemberViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得組隊隊員列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetRideRecord(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.RideRecord}";
                //IEnumerable<RideSimpleRecordView> rideSimpleRecordViews = await this.redisRepository.GetCache<IEnumerable<RideSimpleRecordView>>(AppSettingHelper.Appsetting.Redis.RideDB, cacheKey).ConfigureAwait(false);
                IEnumerable<RideSimpleRecordView> rideSimpleRecordViews = null;
                if (rideSimpleRecordViews == null)
                {
                    IEnumerable<RideDao> rideDaos = await this.rideRepository.GetRecordList(memberID).ConfigureAwait(false);
                    rideDaos = rideDaos.OrderByDescending(dao => Convert.ToDateTime(dao.CreateDate));
                    rideSimpleRecordViews = this.mapper.Map<IEnumerable<RideSimpleRecordView>>(rideDaos);
                    //this.redisRepository.SetCache(AppSettingHelper.Appsetting.Redis.RideDB, cacheKey, JsonConvert.SerializeObject(rideSimpleRecordViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = rideSimpleRecordViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得騎乘記錄發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 回覆組隊騎乘
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ReplyRideGroup(ReplyRideGroupContent content, string memberID)
        {
            try
            {
                #region 發送【回覆組隊騎乘】指令至後端

                ReplyRideGroupRequest request = new ReplyRideGroupRequest()
                {
                    MemberID = memberID,
                    Action = (int)content.Reply,
                };
                CommandData<ReplyRideGroupResponse> response = await this.serverService.DoAction<ReplyRideGroupResponse>((int)RideCommandIDType.ReplyRideGroup, CommandType.Ride, request).ConfigureAwait(false);
                this.logger.LogInfo("回覆組隊騎乘結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)ReplyRideGroupResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.ReplySuccess.ToString()
                        };

                    case (int)ReplyRideGroupResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.ReplyFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【回覆組隊騎乘】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("回覆組隊騎乘發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 發送組隊騎乘通知
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> SendNotify(string memberID)
        {
            try
            {
                #region 發送【通知隊友】指令至後端

                RideGroupNotifyRequest request = new RideGroupNotifyRequest()
                {
                    MemberID = memberID
                };
                CommandData<RideGroupNotifyResponse> response = await this.serverService.DoAction<RideGroupNotifyResponse>((int)RideCommandIDType.UpdateCoordinate, CommandType.Ride, request).ConfigureAwait(false);
                this.logger.LogInfo("發送組隊騎乘通知結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)NotifyRideGroupMemberResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.NotifySuccess.ToString()
                        };

                    case (int)NotifyRideGroupMemberResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.NotifyFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【通知隊友】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送組隊騎乘通知發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 更新組隊騎乘
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="action">action</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdateRideGroup(UpdateRideGroupContent content, string memberID, ActionType action)
        {
            try
            {
                #region 發送【更新組隊騎乘】指令至後端

                UpdateRideGroupRequest request = new UpdateRideGroupRequest()
                {
                    MemberID = memberID,
                    Action = (int)action,
                    InviteList = JsonConvert.SerializeObject(content.MemberIDs)
                };
                CommandData<UpdateRideGroupResponse> response = await this.serverService.DoAction<UpdateRideGroupResponse>((int)RideCommandIDType.UpdateRideGroup, CommandType.Ride, request).ConfigureAwait(false);
                this.logger.LogInfo("更新組隊騎乘結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)UpdateRideGroupResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)UpdateRideGroupResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    case (int)UpdateRideGroupResultType.Repeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【更新組隊騎乘】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新組隊騎乘發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 更新組隊騎乘座標
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdateRideGroupCoordinate(UpdateRideGroupCoordinateContent content, string memberID)
        {
            try
            {
                #region 發送【更新座標】指令至後端

                UpdateRideGroupCoordinateRequest request = new UpdateRideGroupCoordinateRequest()
                {
                    MemberID = memberID,
                    CoordinateX = content.CoordinateX,
                    CoordinateY = content.CoordinateY
                };
                CommandData<UpdateRideGroupCoordinateResponse> response = await this.serverService.DoAction<UpdateRideGroupCoordinateResponse>((int)RideCommandIDType.UpdateCoordinate, CommandType.Ride, request).ConfigureAwait(false);
                this.logger.LogInfo("更新組隊騎乘座標結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)UpdateCoordinateResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)UpdateCoordinateResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【更新座標】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新組隊騎乘座標發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }
    }
}