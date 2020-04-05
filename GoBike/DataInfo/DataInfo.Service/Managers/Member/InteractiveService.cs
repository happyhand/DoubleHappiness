using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Enums;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Models.Member;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Member.View;
using DataInfo.Service.Models.Response;
using FluentValidation.Results;
using Newtonsoft.Json;
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
        /// redisRepository
        /// </summary>
        protected readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="interactiveRepository">interactiveRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public InteractiveService(IMapper mapper, IMemberRepository memberRepository, IInteractiveRepository interactiveRepository, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.memberRepository = memberRepository;
            this.interactiveRepository = interactiveRepository;
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 轉換為會員簡易資訊可視資料
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>MemberSimpleInfoViewDtos</returns>
        private async Task<IEnumerable<MemberSimpleInfoViewDto>> TransformMemberModel(IEnumerable<string> memberIDs)
        {
            this.logger.LogInfo("轉換為會員簡易資訊可視資料", $"MemberIDs: {JsonConvert.SerializeObject(memberIDs)}", null);
            List<MemberSimpleInfoViewDto> memberSimpleInfoViewDtos = new List<MemberSimpleInfoViewDto>();
            if (memberIDs.Any())
            {
                IEnumerable<MemberModel> memberModels = await this.memberRepository.Get(memberIDs).ConfigureAwait(false);
                foreach (MemberModel memberModel in memberModels)
                {
                    string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{AppSettingHelper.Appsetting.Redis.Flag.LastLogin}-{memberModel.MemberID}";
                    MemberSimpleInfoViewDto memberSimpleInfoViewDto = this.mapper.Map<MemberSimpleInfoViewDto>(memberModel);
                    memberSimpleInfoViewDto.OnlineType = await this.redisRepository.IsExist(cacheKey).ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
                    memberSimpleInfoViewDtos.Add(memberSimpleInfoViewDto);
                }
            }

            return memberSimpleInfoViewDtos;
        }

        /// <summary>
        /// 取得好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> GetFriendList(string memberID)
        {
            try
            {
                IEnumerable<InteractiveModel> interactiveModelsOfFriendList = await this.interactiveRepository.GetFriendList(memberID).ConfigureAwait(false);
                IEnumerable<InteractiveModel> interactiveModelsOfBeFriendList = await this.interactiveRepository.GetBeFriendList(memberID).ConfigureAwait(false);
                IEnumerable<string> targetIDsOfFriendList = interactiveModelsOfFriendList.Select(data => data.TargetID);
                IEnumerable<string> creatorIDsOfBeFriendList = interactiveModelsOfBeFriendList
                                                               .Where(data => !targetIDsOfFriendList.Contains(data.CreatorID)) //// 排除掉已經互為好友的會員
                                                               .Select(data => data.CreatorID);
                List<IEnumerable<MemberSimpleInfoViewDto>> memberSimpleInfoViewDtos = new List<IEnumerable<MemberSimpleInfoViewDto>>
                {
                    await this.TransformMemberModel(targetIDsOfFriendList).ConfigureAwait(false),
                    await this.TransformMemberModel(creatorIDsOfBeFriendList).ConfigureAwait(false)
                };
                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = memberSimpleInfoViewDtos
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得好友列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResultDto()
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
        public async Task<ResponseResultDto> UpdateInteractive(string memberID, InteractiveContent content, int status)
        {
            try
            {
                #region 驗證資料

                InteractiveContentValidator interactiveContentValidator = new InteractiveContentValidator();
                ValidationResult validationResult = interactiveContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("更新互動資料結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                if (memberID.Equals(content.TargetID))
                {
                    this.logger.LogWarn("更新互動資料結果", $"Result: 更新失敗，無法指定會員本身為好友 MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "更新資料失敗."
                    };
                }

                #endregion 驗證資料

                #region 取得會員資料

                MemberModel memberModel = (await this.memberRepository.Get(memberID, false).ConfigureAwait(false)).FirstOrDefault();
                if (memberModel == null)
                {
                    this.logger.LogWarn("更新互動資料結果", $"Result: 更新失敗，無會員資料 MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "無會員資料."
                    };
                }

                MemberModel targetModel = (await this.memberRepository.Get(content.TargetID, false).ConfigureAwait(false)).FirstOrDefault();
                if (targetModel == null)
                {
                    this.logger.LogWarn("更新互動資料結果", $"Result: 更新失敗，無目標資料 MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "無會員資料."
                    };
                }

                #endregion 取得會員資料

                #region 新增或更新互動資料

                InteractiveModel interactiveModel = await this.interactiveRepository.Get(memberModel.MemberID, content.TargetID).ConfigureAwait(false);
                bool result = false;
                if (interactiveModel == null)
                {
                    interactiveModel = new InteractiveModel() { CreatorID = memberID, TargetID = content.TargetID, Status = status };
                    result = await this.interactiveRepository.Create(interactiveModel).ConfigureAwait(false);
                }
                else
                {
                    interactiveModel.Status = status;
                    result = await this.interactiveRepository.Update(interactiveModel).ConfigureAwait(false);
                }

                if (!result)
                {
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UpdateFail,
                        Content = "更新資料失敗."
                    };
                }

                #endregion 新增或更新互動資料

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "更新資料成功."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新互動資料發生錯誤", $"MemberID: {memberID} TargetID: {content.TargetID} Status: {status}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "更新資料發生錯誤."
                };
            }
        }
    }
}