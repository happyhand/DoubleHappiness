using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dto.Interactive.Content;
using DataInfo.Core.Models.Dto.Interactive.Request;
using DataInfo.Core.Models.Dto.Interactive.Response;
using DataInfo.Core.Models.Dto.Member.View;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Repository.Interfaces.Interactive;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Service.Interfaces.Interactive;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Interfaces.Server;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Interactive
{
    /// <summary>
    /// 互動服務
    /// </summary>
    public class InteractiveService : IInteractiveService
    {
        /// <summary>
        /// rideRepository
        /// </summary>
        private readonly IInteractiveRepository interactiveRepository;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("InteractiveService");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        private readonly IMemberRepository memberRepository;

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="memberService">memberService</param>
        /// <param name="serverService">serverService</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="interactiveRepository">interactiveRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public InteractiveService(IMapper mapper, IMemberService memberService, IServerService serverService, IMemberRepository memberRepository, IInteractiveRepository interactiveRepository, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.memberService = memberService;
            this.serverService = serverService;
            this.memberRepository = memberRepository;
            this.interactiveRepository = interactiveRepository;
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 取得黑名單列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetBlackList(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Interactive}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.Black}";
                IEnumerable<MemberSimpleInfoView> memberSimpleInfoViews = await this.redisRepository.GetCache<IEnumerable<MemberSimpleInfoView>>(cacheKey).ConfigureAwait(false);
                if (memberSimpleInfoViews == null)
                {
                    IEnumerable<string> blackIDList = await this.interactiveRepository.GetBlackList(memberID).ConfigureAwait(false);
                    IEnumerable<MemberDao> blackDaoList = await memberRepository.Get(blackIDList, null).ConfigureAwait(false);
                    memberSimpleInfoViews = await this.memberService.TransformMemberSimpleInfoView(blackDaoList).ConfigureAwait(false);
                    this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(memberSimpleInfoViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = memberSimpleInfoViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得黑名單列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetFriendList(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Interactive}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.Friend}";
                List<IEnumerable<MemberSimpleInfoView>> memberSimpleInfoViews = await this.redisRepository.GetCache<List<IEnumerable<MemberSimpleInfoView>>>(cacheKey).ConfigureAwait(false);
                if (memberSimpleInfoViews == null)
                {
                    Task<IEnumerable<string>> friendIDListTask = this.interactiveRepository.GetFriendList(memberID);
                    Task<IEnumerable<string>> beFriendIDListTask = this.interactiveRepository.GetBeFriendList(memberID);

                    IEnumerable<string> friendIDList = await friendIDListTask.ConfigureAwait(false);
                    IEnumerable<string> beFriendIDList = await beFriendIDListTask.ConfigureAwait(false);
                    beFriendIDList = beFriendIDList.Where(id => !friendIDList.Contains(id));

                    Task<IEnumerable<MemberDao>> friendDaoListTask = this.memberRepository.Get(friendIDList, null);
                    Task<IEnumerable<MemberDao>> beFriendDaoListTask = this.memberRepository.Get(beFriendIDList, null);

                    IEnumerable<MemberDao> friendDaoList = await friendDaoListTask.ConfigureAwait(false);
                    IEnumerable<MemberDao> beFriendDaoList = await beFriendDaoListTask.ConfigureAwait(false);

                    memberSimpleInfoViews = new List<IEnumerable<MemberSimpleInfoView>>
                    {
                        await this.memberService.TransformMemberSimpleInfoView(friendDaoList).ConfigureAwait(false),
                        await this.memberService.TransformMemberSimpleInfoView(beFriendDaoList).ConfigureAwait(false),
                    };
                    this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(memberSimpleInfoViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = memberSimpleInfoViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得好友列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 更新互動資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="status">status</param>
        /// <param name="action">action</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdateInteractive(InteractiveContent content, string memberID, InteractiveType status, ActionType action)
        {
            try
            {
                #region 驗證資料

                if (content.MemberID.Equals(memberID))
                {
                    this.logger.LogWarn("更新互動資料失敗，互動對象無法為會員本身", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} Status: {status} Action: {action}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = ResponseErrorMessageType.TargetSelfError.ToString()
                    };
                }

                #endregion 驗證資料

                #region 發送【更新朋友列表】指令至後端

                CommandData<UpdateInteractiveResponse> response = null;
                switch (status)
                {
                    case InteractiveType.Friend:
                        response = await this.serverService.DoAction<UpdateInteractiveResponse>((int)UserCommandIDType.UpdateFriendList, CommandType.User, new UpdateFriendListRequest()
                        {
                            Action = (int)action,
                            MemberID = memberID,
                            FriendID = content.MemberID
                        }).ConfigureAwait(false);
                        break;

                    case InteractiveType.Black:
                        response = await this.serverService.DoAction<UpdateInteractiveResponse>((int)UserCommandIDType.UpdateBlackList, CommandType.User, new UpdateBlackListRequest()
                        {
                            Action = (int)action,
                            MemberID = memberID,
                            BlackID = content.MemberID
                        }).ConfigureAwait(false);
                        break;

                    default:
                        this.logger.LogWarn("更新互動資料失敗，會員互動類別設定錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} Status: {status} Action: {action}", null);
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };
                }

                this.logger.LogInfo("更新互動資料結果", $"Response: {JsonConvert.SerializeObject(response)} MemberID: { memberID} Content: {JsonConvert.SerializeObject(content)} Status: { status} Action: { action}", null);

                switch (response.Data.Result)
                {
                    case (int)UpdateInteractiveResultType.Success:

                        #region 刪除 Interactive 的 Redis

                        string subFlag = status.Equals(InteractiveType.Friend) ? AppSettingHelper.Appsetting.Redis.SubFlag.Friend : AppSettingHelper.Appsetting.Redis.SubFlag.Black;
                        string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Interactive}-{memberID}-{subFlag}";
                        this.redisRepository.DeleteCache(cacheKey);

                        #endregion 刪除 Interactive 的 Redis

                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)UpdateInteractiveResultType.Fail:
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

                #endregion 發送【更新朋友列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新互動資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} Status: {status} Action: {action}", ex);
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