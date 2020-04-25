using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Member.View;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Repository.Interfaces;
using DataInfo.Service.Interfaces.Member;
using FluentValidation.Results;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Member
{
    /// <summary>
    /// 互動服務
    /// </summary>
    public class InteractiveService : IInteractiveService
    {
        /// <summary>
        /// rideRepository
        /// </summary>
        protected readonly IInteractiveRepository interactiveRepository;

        /// <summary>
        /// logger
        /// </summary>
        protected readonly ILogger logger = LogManager.GetLogger("InteractiveService");

        /// <summary>
        /// mapper
        /// </summary>
        protected readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        protected readonly IMemberRepository memberRepository;

        /// <summary>
        /// memberService
        /// </summary>
        protected readonly IMemberService memberService;

        /// <summary>
        /// redisRepository
        /// </summary>
        protected readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="memberService">memberService</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="interactiveRepository">interactiveRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public InteractiveService(IMapper mapper, IMemberService memberService, IMemberRepository memberRepository, IInteractiveRepository interactiveRepository, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.memberService = memberService;
            this.memberRepository = memberRepository;
            this.interactiveRepository = interactiveRepository;
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 刪除互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResult> DeleteInteractive(string memberID, InteractiveContent content)
        {
            try
            {
                //// TODO
                //#region 驗證資料

                //InteractiveContentValidator interactiveContentValidator = new InteractiveContentValidator();
                //ValidationResult validationResult = interactiveContentValidator.Validate(content);
                //if (!validationResult.IsValid)
                //{
                //    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                //    this.logger.LogWarn("刪除互動資料結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} TargetID: {content.TargetID}", null);
                //    return new ResponseResult()
                //    {
                //        Result = false,
                //        ResultCode = (int)ResponseResultType.InputError,
                //        Content = errorMessgae
                //    };
                //}

                //#endregion 驗證資料

                //#region 刪除互動資料

                //bool result = await this.interactiveRepository.Delete(memberID, content.TargetID).ConfigureAwait(false);
                //if (!result)
                //{
                //    return new ResponseResult()
                //    {
                //        Result = false,
                //        ResultCode = (int)ResponseResultType.DeleteFail,
                //        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                //    };
                //}

                //return new ResponseResult()
                //{
                //    Result = true,
                //    ResultCode = (int)ResponseResultType.Success,
                //    Content = MessageHelper.Message.ResponseMessage.Update.Success
                //};

                //#endregion 刪除互動資料

                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除互動資料發生錯誤", $"MemberID: {memberID} TargetID: {content.TargetID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 取得黑名單列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResult> GetBlackList(string memberID)
        {
            try
            {
                //// TODO
                //IEnumerable<InteractiveModel> blackList = await this.interactiveRepository.GetBlackList(memberID).ConfigureAwait(false);
                //IEnumerable<string> memberIDs = blackList.Select(data => data.TargetID);
                //IEnumerable<MemberSimpleInfoView> memberSimpleInfoViews = await this.memberService.TransformMemberSimpleInfoView(null, memberIDs).ConfigureAwait(false);
                //return new ResponseResult()
                //{
                //    Result = true,
                //    ResultCode = (int)ResponseResultType.Success,
                //    Content = memberSimpleInfoViews
                //};

                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得黑名單列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                };
            }
        }

        /// <summary>
        /// 取得好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResult> GetFriendList(string memberID)
        {
            try
            {
                //// TODO
                //IEnumerable<InteractiveModel> interactiveModelsOfFriendList = await this.interactiveRepository.GetFriendList(memberID).ConfigureAwait(false);
                //IEnumerable<InteractiveModel> interactiveModelsOfBeFriendList = await this.interactiveRepository.GetBeFriendList(memberID).ConfigureAwait(false);
                //IEnumerable<string> targetIDsOfFriendList = interactiveModelsOfFriendList.Select(data => data.TargetID);
                //IEnumerable<string> creatorIDsOfBeFriendList = interactiveModelsOfBeFriendList
                //                                               .Where(data => !targetIDsOfFriendList.Contains(data.CreatorID)) //// 排除掉已經互為好友的會員
                //                                               .Select(data => data.CreatorID);
                //List<IEnumerable<MemberSimpleInfoView>> memberSimpleInfoViews = new List<IEnumerable<MemberSimpleInfoView>>
                //{
                //    await this.memberService.TransformMemberSimpleInfoView(null, targetIDsOfFriendList).ConfigureAwait(false),
                //    await this.memberService.TransformMemberSimpleInfoView(null, creatorIDsOfBeFriendList).ConfigureAwait(false)
                //};
                //return new ResponseResult()
                //{
                //    Result = true,
                //    ResultCode = (int)ResponseResultType.Success,
                //    Content = memberSimpleInfoViews
                //};

                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得好友列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                };
            }
        }

        /// <summary>
        /// 更新互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <param name="status">status</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResult> UpdateInteractive(string memberID, InteractiveContent content, int status)
        {
            try
            {
                //// TODO
                //#region 驗證資料

                //InteractiveContentValidator interactiveContentValidator = new InteractiveContentValidator();
                //ValidationResult validationResult = interactiveContentValidator.Validate(content);
                //if (!validationResult.IsValid)
                //{
                //    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                //    this.logger.LogWarn("更新互動資料結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", null);
                //    return new ResponseResult()
                //    {
                //        Result = false,
                //        ResultCode = (int)ResponseResultType.InputError,
                //        Content = errorMessgae
                //    };
                //}

                //if (memberID.Equals(content.TargetID))
                //{
                //    this.logger.LogWarn("更新互動資料結果", $"Result: 更新失敗，無法指定會員本身為好友 MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", null);
                //    return new ResponseResult()
                //    {
                //        Result = false,
                //        ResultCode = (int)ResponseResultType.InputError,
                //        Content = "更新資料失敗."
                //    };
                //}

                //#endregion 驗證資料

                //#region 取得會員資料

                //MemberDao creatorDao = (await this.memberRepository.Get(memberID, false).ConfigureAwait(false)).FirstOrDefault();
                //if (creatorDao == null)
                //{
                //    this.logger.LogWarn("更新互動資料結果", $"Result: 更新失敗，無會員資料 MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", null);
                //    return new ResponseResult()
                //    {
                //        Result = false,
                //        ResultCode = (int)ResponseResultType.InputError,
                //        Content = "無會員資料."
                //    };
                //}

                //MemberDao targetDao = (await this.memberRepository.Get(content.TargetID, false).ConfigureAwait(false)).FirstOrDefault();
                //if (targetDao == null)
                //{
                //    this.logger.LogWarn("更新互動資料結果", $"Result: 更新失敗，無目標資料 MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", null);
                //    return new ResponseResult()
                //    {
                //        Result = false,
                //        ResultCode = (int)ResponseResultType.InputError,
                //        Content = "無會員資料."
                //    };
                //}

                //#endregion 取得會員資料

                //#region 新增或更新互動資料

                //InteractiveModel interactiveModel = await this.interactiveRepository.Get(creatorDao.MemberID, targetDao.MemberID).ConfigureAwait(false);
                //bool result = false;
                //if (interactiveModel == null)
                //{
                //    interactiveModel = new InteractiveModel() { CreatorID = creatorDao.MemberID, TargetID = targetDao.MemberID, Status = status };
                //    result = await this.interactiveRepository.Create(interactiveModel).ConfigureAwait(false);
                //}
                //else
                //{
                //    interactiveModel.Status = status;
                //    result = await this.interactiveRepository.Update(interactiveModel).ConfigureAwait(false);
                //}

                //if (!result)
                //{
                //    return new ResponseResult()
                //    {
                //        Result = false,
                //        ResultCode = (int)ResponseResultType.UpdateFail,
                //        Content = "更新資料失敗."
                //    };
                //}

                //#endregion 新增或更新互動資料

                //return new ResponseResult()
                //{
                //    Result = true,
                //    ResultCode = (int)ResponseResultType.Success,
                //    Content = "更新資料成功."
                //};

                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "更新資料發生錯誤."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新互動資料發生錯誤", $"MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "更新資料發生錯誤."
                };
            }
        }
    }
}