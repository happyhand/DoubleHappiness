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
using DataInfo.Repository.Interfaces;
using DataInfo.Service.Interfaces.Interactive;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Interfaces.Server;
using FluentValidation.Results;
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
                IEnumerable<string> blackIDList = await this.interactiveRepository.GetBlackList(memberID).ConfigureAwait(false);
                IEnumerable<MemberDao> blackDaoList = await memberRepository.Get(blackIDList, null).ConfigureAwait(false);
                IEnumerable<MemberSimpleInfoView> memberSimpleInfoViews = await this.memberService.TransformMemberSimpleInfoView(blackDaoList).ConfigureAwait(false);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = memberSimpleInfoViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得黑名單列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
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
                Task<IEnumerable<string>> friendIDListTask = this.interactiveRepository.GetFriendList(memberID);
                Task<IEnumerable<string>> beFriendIDListTask = this.interactiveRepository.GetBeFriendList(memberID);

                IEnumerable<string> friendIDList = await friendIDListTask.ConfigureAwait(false);
                IEnumerable<string> beFriendIDList = await beFriendIDListTask.ConfigureAwait(false);
                beFriendIDList = beFriendIDList.Where(id => !friendIDList.Contains(id));

                Task<IEnumerable<MemberDao>> friendDaoListTask = this.memberRepository.Get(friendIDList, null);
                Task<IEnumerable<MemberDao>> beFriendDaoListTask = this.memberRepository.Get(beFriendIDList, null);

                IEnumerable<MemberDao> friendDaoList = await friendDaoListTask.ConfigureAwait(false);
                IEnumerable<MemberDao> beFriendDaoList = await beFriendDaoListTask.ConfigureAwait(false);

                List<IEnumerable<MemberSimpleInfoView>> memberSimpleInfoViews = new List<IEnumerable<MemberSimpleInfoView>>
                {
                    await this.memberService.TransformMemberSimpleInfoView(friendDaoList).ConfigureAwait(false),
                    await this.memberService.TransformMemberSimpleInfoView(beFriendDaoList).ConfigureAwait(false),
                };
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = memberSimpleInfoViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得好友列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 更新互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <param name="status">status</param>
        /// <param name="action">action</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdateInteractive(string memberID, InteractiveContent content, InteractiveType status, InteractiveActionType action)
        {
            try
            {
                #region 驗證資料

                InteractiveContentValidator interactiveContentValidator = new InteractiveContentValidator(memberID);
                ValidationResult validationResult = interactiveContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("更新互動資料結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} TargetID: {content.TargetID} Status: {status} Action: {action}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
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
                            FriendID = content.TargetID
                        }).ConfigureAwait(false);
                        break;

                    case InteractiveType.Black:
                        response = await this.serverService.DoAction<UpdateInteractiveResponse>((int)UserCommandIDType.UpdateBlackList, CommandType.User, new UpdateBlackListRequest()
                        {
                            Action = (int)action,
                            MemberID = memberID,
                            BlackID = content.TargetID
                        }).ConfigureAwait(false);
                        break;

                    default:
                        this.logger.LogWarn("更新互動資料結果", $"Result: 會員互動類別設定錯誤 MemberID: {memberID} TargetID: {content.TargetID} Status: {status} Action: {action}", null);
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.InputError,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };
                }

                this.logger.LogInfo("更新互動資料結果", $"Result: {response.Data.Result} MemberID: { memberID} TargetID: { content.TargetID} Status: { status} Action: { action}", null);

                switch (response.Data.Result)
                {
                    case (int)UpdateInteractiveResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)UpdateInteractiveResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.CreateFail,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };
                }

                #endregion 發送【更新朋友列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新互動資料發生錯誤", $"MemberID: {memberID} TargetID: {content.TargetID} Status: {status} Action: {action}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }
    }
}