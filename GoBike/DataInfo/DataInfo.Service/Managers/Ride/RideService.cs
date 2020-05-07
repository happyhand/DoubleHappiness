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
using DataInfo.Repository.Interfaces;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Ride;
using DataInfo.Service.Interfaces.Server;
using FluentValidation.Results;
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
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// uploadService
        /// </summary>
        private readonly IUploadService uploadService;

        /// <summary>
        /// interactiveRepository
        /// </summary>
        protected readonly IInteractiveRepository interactiveRepository;

        /// <summary>
        /// logger
        /// </summary>
        protected readonly ILogger logger = LogManager.GetLogger("RideService");

        /// <summary>
        /// mapper
        /// </summary>
        protected readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        protected readonly IMemberRepository memberRepository;

        /// <summary>
        /// rideRepository
        /// </summary>
        protected readonly IRideRepository rideRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="uploadService">uploadService</param>
        /// <param name="serverService">serverService</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="interactiveRepository">interactiveRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public RideService(IMapper mapper, IUploadService uploadService, IServerService serverService, IMemberRepository memberRepository, IInteractiveRepository interactiveRepository, IRideRepository rideRepository)
        {
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.serverService = serverService;
            this.memberRepository = memberRepository;
            this.interactiveRepository = interactiveRepository;
            this.rideRepository = rideRepository;
        }

        #region 騎乘資料

        ///// <summary>
        ///// 騎乘資料更新處理
        ///// </summary>
        ///// <param name="content">content</param>
        ///// <param name="rideModel">rideModel</param>
        ///// <returns>string</returns>
        //private async Task<string> UpdateInfoHandler(RideUpdateInfoContent content, RideModel rideModel)
        //{
        //    if (content.County != (int)CountyType.None)
        //    {
        //        rideModel.County = content.County;
        //    }

        // if (content.Level != (int)RideLevelType.None) { rideModel.Level = content.Level; }

        // if (content.Route != null && content.Route.Any()) { rideModel.Route =
        // JsonConvert.SerializeObject(content.Route); }

        // if (content.ShareContent != null || !string.IsNullOrEmpty(content.Photo)) { List<string>
        // imgBase64s = new List<string>(); if (content.ShareContent != null) {
        // imgBase64s.AddRange(content.ShareContent.Select(data => data.ElementAt(1))); }

        // if (!string.IsNullOrEmpty(content.Photo)) { imgBase64s.Add(content.Photo); }

        // ResponseResult uploadResponseResult = await
        // this.uploadService.UploadImages(imgBase64s).ConfigureAwait(false); if
        // (!uploadResponseResult.Result) { return "上傳圖片失敗."; }

        // IEnumerable<string> imgUrls = uploadResponseResult.Content; if (content.ShareContent !=
        // null && content.ShareContent.Any()) { content.ShareContent =
        // content.ShareContent.Select((data, index) => { string imgUrl = imgUrls.ElementAt(index);
        // return new List<string>() { data.ElementAt(0), imgUrls.ElementAt(index) }; });
        // rideModel.ShareContent = JsonConvert.SerializeObject(content.ShareContent); }

        // if (!string.IsNullOrEmpty(content.Photo)) { rideModel.Photo = imgUrls.LastOrDefault(); } }

        // if (!string.IsNullOrEmpty(content.Title)) { rideModel.Title = content.Title; }

        //    return string.Empty;
        //}

        /// <summary>
        /// 新增騎乘資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> AddRideData(string memberID, AddRideInfoContent content)
        {
            try
            {
                #region 驗證騎乘資料

                AddRideInfoContentValidator addRideInfoContentValidator = new AddRideInfoContentValidator();
                ValidationResult validationResult = addRideInfoContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("新增騎乘資料結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                if (content.ShareContent == null)
                {
                    content.ShareContent = new string[][] { };
                }

                if (string.IsNullOrEmpty(content.Title))
                {
                    content.Title = $"{DateTime.UtcNow:yyyy/MM/dd}";
                }

                #endregion 驗證騎乘資料

                #region 上傳圖片

                List<string> imgBase64s = content.ShareContent.Select(data => data.ElementAt(1)).ToList();
                imgBase64s.Add(content.Photo);
                IEnumerable<string> imgUris = await this.uploadService.UploadRideImages(imgBase64s, true).ConfigureAwait(false);
                if (imgUris == null || !imgUris.Any())
                {
                    this.logger.LogWarn("新增騎乘資料結果", $"Result: 上傳圖片失敗 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = MessageHelper.Message.ResponseMessage.Upload.PhotoFail
                    };
                }

                content.ShareContent = content.ShareContent.Select((data, index) =>
                {
                    string imgUrl = imgUris.ElementAt(index);
                    if (string.IsNullOrEmpty(imgUrl))
                    {
                        this.logger.LogWarn("新增騎乘資料結果", $"Result: 騎乘分享內容圖片轉換失敗 MemberID: {memberID} Data: {JsonConvert.SerializeObject(data)}", null);
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
                this.logger.LogInfo("新增騎乘資料結果", $"Result: {response.Data.Result} MemberID: {memberID} Request: {JsonConvert.SerializeObject(request)} Content: {JsonConvert.SerializeObject(content)}", null);

                switch (response.Data.Result)
                {
                    case (int)CreateRideRecordResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Add.Success
                        };

                    case (int)CreateRideRecordResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.CreateFail,
                            Content = MessageHelper.Message.ResponseMessage.Add.Fail
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Add.Fail
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
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Add.Fail
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
                IEnumerable<string> friendIDList = await this.interactiveRepository.GetFriendList(memberID).ConfigureAwait(false);
                Task<IEnumerable<RideDistanceDao>> rideDistanceDaosTask = this.rideRepository.GetWeekDistance(friendIDList);
                Task<IEnumerable<MemberDao>> friendDaosTask = this.memberRepository.Get(friendIDList, null);

                Dictionary<string, RideDistanceDao> rideDistanceMap = (await rideDistanceDaosTask.ConfigureAwait(false)).ToDictionary(data => data.MemberID);
                IEnumerable<MemberDao> friendDaos = await friendDaosTask.ConfigureAwait(false);
                IEnumerable<RideFriendWeekRankView> rideFriendWeekRankViews = friendDaos.Select(data =>
                {
                    rideDistanceMap.TryGetValue(data.MemberID, out RideDistanceDao rideDistanceDao);
                    RideFriendWeekRankView rideFriendWeekRankView = new RideFriendWeekRankView()
                    {
                        Avatar = data.Avatar,
                        Nickname = data.Nickname
                    };
                    if (rideDistanceDao != null)
                    {
                        rideFriendWeekRankView.WeekDistance = rideDistanceDao.WeekDistance;
                    }

                    return rideFriendWeekRankView;
                });

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = rideFriendWeekRankViews.OrderByDescending(data => data.WeekDistance)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得好友週里程排名發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        ///// <summary>
        ///// 取得騎乘資料
        ///// </summary>
        ///// <param name="rideID">rideID</param>
        ///// <returns>ResponseResult</returns>
        //public async Task<ResponseResult> GetRideData(string rideID)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(rideID))
        //        {
        //            this.logger.LogWarn("取得騎乘資料失敗", $"Result: 騎乘編號無效", null);
        //            return new ResponseResult()
        //            {
        //                Result = false,
        //                Content = "騎乘編號無效."
        //            };
        //        }

        // RideModel rideModel = await this.rideRepository.Get(rideID); if (rideModel == null) {
        // this.logger.LogWarn("取得騎乘資料失敗", $"Result: 無騎乘資料 RideID: {rideID}", null); return new
        // ResponseResult() { Result = false, Content = "無騎乘資料." }; }

        //        return new ResponseResult()
        //        {
        //            Result = true,
        //            Content = this.mapper.Map<RideInfoView>(rideModel) //// TODO 待轉換客端所需資料
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("取得騎乘資料發生錯誤", $"RideID: {rideID}", ex);
        //        return new ResponseResult()
        //        {
        //            Result = false,
        //            Content = "取得資料發生錯誤."
        //        };
        //    }
        //}

        ///// <summary>
        ///// 取得會員的騎乘資料列表
        ///// </summary>
        ///// <param name="memberID">memberID</param>
        ///// <returns>ResponseResult</returns>
        //public async Task<ResponseResult> GetRideDataListOfMember(string memberID)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(memberID))
        //        {
        //            this.logger.LogWarn("取得會員的騎乘資料列表失敗", $"Result: 會員編號無效", null);
        //            return new ResponseResult()
        //            {
        //                Result = false,
        //                ResultCode = (int)ResponseResultType.InputError,
        //                Content = "會員編號無效."
        //            };
        //        }

        //        IEnumerable<RideModel> rideModels = await this.rideRepository.GetListOfMember(memberID);
        //        return new ResponseResult()
        //        {
        //            Result = true,
        //            ResultCode = (int)ResponseResultType.Success,
        //            Content = this.mapper.Map<IEnumerable<RideInfoView>>(rideModels) //// TODO 待轉換客端所需資料
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("取得會員的騎乘資料列表發生錯誤", $"MemberID: {memberID}", ex);
        //        return new ResponseResult()
        //        {
        //            Result = false,
        //            ResultCode = (int)ResponseResultType.UnknownError,
        //            Content = "取得資料發生錯誤."
        //        };
        //    }
        //}

        ///// <summary>
        ///// 更新騎乘資料 (TODO)
        ///// </summary>
        ///// <param name="memberID">memberID</param>
        ///// <param name="content">content</param>
        ///// <returns>ResponseResultDto</returns>
        //public async Task<ResponseResultDto> UpdateRideData(string memberID, RideUpdateInfoContent content)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(memberID))
        //        {
        //            return new ResponseResultDto()
        //            {
        //                Result = false,
        //                ResultCode = (int)ResponseResultType.InputError,
        //                Content = "會員編號無效."
        //            };
        //        }

        // if (string.IsNullOrEmpty(content.RideID)) { return new ResponseResultDto() { Result =
        // false, ResultCode = (int)ResponseResultType.InputError, Content = "騎乘資料編號無效." }; }

        // RideModel rideModel = await
        // this.rideRepository.Get(content.RideID).ConfigureAwait(false); if (rideModel == null) {
        // this.logger.LogWarn("更新騎乘資料結果", $"Result: 搜尋失敗，無騎乘資料 MemberID: {memberID} Content:
        // {JsonConvert.SerializeObject(content)}", null); return new ResponseResultDto() { Result =
        // false, ResultCode = (int)ResponseResultType.InputError, Content = "無騎乘資料." }; }

        // string updateInfoHandlerResult = await this.UpdateInfoHandler(content,
        // rideModel).ConfigureAwait(false); if (!string.IsNullOrEmpty(updateInfoHandlerResult)) {
        // this.logger.LogWarn("更新騎乘資料結果", $"Result: 更新失敗({updateInfoHandlerResult}) MemberID:
        // {memberID} Content: {JsonConvert.SerializeObject(content)}", null); return new
        // ResponseResultDto() { Result = false, ResultCode = (int)ResponseResultType.UpdateFail,
        // Content = updateInfoHandlerResult }; }

        // bool isSuccess = await this.rideRepository.Update(rideModel).ConfigureAwait(false);
        // this.logger.LogInfo("更新騎乘資料結果", $"Result: {isSuccess} MemberID: {memberID} Content:
        // {JsonConvert.SerializeObject(content)} RideModel:
        // {JsonConvert.SerializeObject(rideModel)}", null); if (!isSuccess) { return new
        // ResponseResultDto() { Result = false, ResultCode = (int)ResponseResultType.UpdateFail,
        // Content = "更新資料失敗." }; }

        //        return new ResponseResultDto()
        //        {
        //            Result = true,
        //            ResultCode = (int)ResponseResultType.Success,
        //            Content = "更新資料成功."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("更新騎乘資料結果發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
        //        return new ResponseResultDto()
        //        {
        //            Result = false,
        //            ResultCode = (int)ResponseResultType.UnknownError,
        //            Content = "更新資料發生錯誤."
        //        };
        //    }
        //}

        #endregion 騎乘資料
    }
}