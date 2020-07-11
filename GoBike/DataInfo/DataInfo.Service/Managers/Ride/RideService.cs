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
using FluentValidation.Results;
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
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

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
        /// 新增騎乘資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> AddRideData(string memberID, AddRideDataContent content)
        {
            try
            {
                #region 驗證騎乘資料

                AddRideDataContentValidator contentValidator = new AddRideDataContentValidator();
                ValidationResult validationResult = contentValidator.Validate(content);
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
                this.logger.LogInfo("新增騎乘資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);

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
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.HomeInfo}";
                IEnumerable<RideFriendWeekRankView> rideFriendWeekRankViews = await this.redisRepository.GetCache<IEnumerable<RideFriendWeekRankView>>(cacheKey).ConfigureAwait(false);
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
                            Nickname = data.Nickname
                        };
                        if (rideDistanceDao != null)
                        {
                            rideFriendWeekRankView.WeekDistance = rideDistanceDao.WeekDistance;
                        }

                        return rideFriendWeekRankView;
                    });

                    rideFriendWeekRankViews = rideFriendWeekRankViews.OrderByDescending(data => data.WeekDistance);
                    this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(rideFriendWeekRankViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
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
        /// 取得騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetRideRecord(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.RideRecord}";
                IEnumerable<RideSimpleRecordView> rideSimpleRecordViews = await this.redisRepository.GetCache<IEnumerable<RideSimpleRecordView>>(cacheKey).ConfigureAwait(false);
                if (rideSimpleRecordViews == null)
                {
                    IEnumerable<RideDao> rideDaos = await this.rideRepository.GetRecordList(memberID).ConfigureAwait(false);
                    rideSimpleRecordViews = this.mapper.Map<IEnumerable<RideSimpleRecordView>>(rideDaos);
                    this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(rideSimpleRecordViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
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
    }
}