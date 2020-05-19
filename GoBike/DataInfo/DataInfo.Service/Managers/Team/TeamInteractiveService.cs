using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Dto.Team.Request;
using DataInfo.Core.Models.Dto.Team.Response;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces;
using DataInfo.Service.Interfaces.Server;
using DataInfo.Service.Interfaces.Team;
using FluentValidation.Results;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Team
{
    /// <summary>
    /// 車隊互動服務
    /// </summary>
    public class TeamInteractiveService : ITeamInteractiveService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamInteractiveService");

        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// teamRepository
        /// </summary>
        private readonly ITeamRepository teamRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="serverService">serverService</param>
        /// <param name="teamRepository">teamRepository</param>
        public TeamInteractiveService(IServerService serverService, ITeamRepository teamRepository)
        {
            this.serverService = serverService;
            this.teamRepository = teamRepository;
        }

        /// <summary>
        /// 會員是否已加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamDao">teamDao</param>
        /// <returns>bool</returns>
        private bool MemberHasJoinTeam(string memberID, TeamDao teamDao)
        {
            return teamDao.Leader.Equals(memberID) || teamDao.TeamViceLeaderIDs.Contains(memberID) || teamDao.TeamMemberIDs.Contains(memberID);
        }

        /// <summary>
        /// 會員是否有車隊權限
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamDao">teamDao</param>
        /// <param name="isHighest">isHighest</param>
        /// <returns>bool</returns>
        private bool MemberHasTeamAuthority(string memberID, TeamDao teamDao, bool isHighest)
        {
            return isHighest ? teamDao.Leader.Equals(memberID) : teamDao.Leader.Equals(memberID) || teamDao.TeamViceLeaderIDs.Contains(memberID);
        }

        /// <summary>
        /// 申請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ApplyJoinTeam(string memberID, TeamApplyJoinContent content)
        {
            try
            {
                #region 驗證資料

                TeamApplyJoinContentValidator teamApplyJoinContentValidator = new TeamApplyJoinContentValidator();
                ValidationResult validationResult = teamApplyJoinContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("申請加入車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (teamDao.ApplyJoinList.Contains(memberID))
                {
                    this.logger.LogWarn("申請加入車隊結果", $"Result: 會員已申請加入車隊，直接回應成功 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = MessageHelper.Message.ResponseMessage.Update.Success
                    };
                }

                if (this.MemberHasJoinTeam(memberID, teamDao))
                {
                    this.logger.LogWarn("申請加入車隊結果", $"Result: 會員已加入車隊 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                    };
                }

                if (teamDao.InviteJoinList.Contains(memberID))
                {
                    this.logger.LogWarn("申請加入車隊結果", $"Result: 會員已被邀請加入車隊 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                    };
                }

                #endregion 驗證資料

                #region 發送【更新申請加入車隊列表】指令至後端

                TeamApplyJoinRequest request = new TeamApplyJoinRequest()
                {
                    MemberID = memberID,
                    Action = (int)ActionType.Add,
                    TeamID = content.TeamID
                };

                CommandData<TeamApplyJoinResponse> response = await this.serverService.DoAction<TeamApplyJoinResponse>((int)TeamCommandIDType.UpdateApplyJoinList, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("申請加入車隊結果", $"Result: {response.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateApplyJoinListResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)UpdateApplyJoinListResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
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

                #endregion 發送【更新申請加入車隊列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("申請加入車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 取消申請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> CancelApplyJoinTeam(string memberID, TeamApplyJoinContent content)
        {
            try
            {
                #region 驗證資料

                TeamApplyJoinContentValidator teamApplyJoinContentValidator = new TeamApplyJoinContentValidator();
                ValidationResult validationResult = teamApplyJoinContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("取消申請加入車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (!teamDao.ApplyJoinList.Contains(memberID))
                {
                    this.logger.LogWarn("取消申請加入車隊結果", $"Result: 會員未申請加入車隊，直接回應成功 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = MessageHelper.Message.ResponseMessage.Update.Success
                    };
                }

                #endregion 驗證資料

                #region 發送【更新申請加入車隊列表】指令至後端

                TeamApplyJoinRequest request = new TeamApplyJoinRequest()
                {
                    MemberID = memberID,
                    Action = (int)ActionType.Delete,
                    TeamID = content.TeamID
                };

                CommandData<TeamApplyJoinResponse> response = await this.serverService.DoAction<TeamApplyJoinResponse>((int)TeamCommandIDType.UpdateApplyJoinList, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("取消申請加入車隊結果", $"Result: {response.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateApplyJoinListResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)UpdateApplyJoinListResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
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

                #endregion 發送【更新申請加入車隊列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("取消申請加入車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 取消邀請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> CancelInviteJoinTeam(string memberID, TeamInviteJoinContent content)
        {
            try
            {
                #region 驗證資料

                TeamInviteJoinContentValidator teamInviteJoinContentValidator = new TeamInviteJoinContentValidator();
                ValidationResult validationResult = teamInviteJoinContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("取消邀請加入車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (!teamDao.InviteJoinList.Contains(content.MemberID))
                {
                    this.logger.LogWarn("取消邀請加入車隊結果", $"Result: 會員未被邀請加入車隊，直接回應成功 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = MessageHelper.Message.ResponseMessage.Update.Success
                    };
                }

                if (!this.MemberHasTeamAuthority(memberID, teamDao, false))
                {
                    this.logger.LogWarn("取消邀請加入車隊結果", $"Result: 無車隊權限 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Team.TeamAuthorityNotEnough
                    };
                }

                #endregion 驗證資料

                #region 發送【更新邀請加入車隊列表】指令至後端

                TeamInviteJoinRequest request = new TeamInviteJoinRequest()
                {
                    MemberID = content.MemberID,
                    Action = (int)ActionType.Delete,
                    TeamID = content.TeamID
                };

                CommandData<TeamInviteJoinResponse> response = await this.serverService.DoAction<TeamInviteJoinResponse>((int)TeamCommandIDType.UpdateInviteJoinList, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("取消邀請加入車隊結果", $"Result: {response.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateInviteJoinListResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)UpdateInviteJoinListResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
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

                #endregion 發送【更新邀請加入車隊列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("取消邀請加入車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 邀請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> InviteJoinTeam(string memberID, TeamInviteJoinContent content)
        {
            try
            {
                #region 驗證資料

                TeamInviteJoinContentValidator teamInviteJoinContentValidator = new TeamInviteJoinContentValidator();
                ValidationResult validationResult = teamInviteJoinContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("邀請加入車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (teamDao.InviteJoinList.Contains(content.MemberID))
                {
                    this.logger.LogWarn("邀請加入車隊結果", $"Result: 會員已被邀請加入車隊，直接回應成功 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = MessageHelper.Message.ResponseMessage.Update.Success
                    };
                }

                if (this.MemberHasJoinTeam(content.MemberID, teamDao))
                {
                    this.logger.LogWarn("邀請加入車隊結果", $"Result: 會員已加入車隊 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                    };
                }

                if (teamDao.ApplyJoinList.Contains(content.MemberID))
                {
                    this.logger.LogWarn("邀請加入車隊結果", $"Result: 會員已申請加入車隊 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                    };
                }

                if (!this.MemberHasTeamAuthority(memberID, teamDao, false))
                {
                    this.logger.LogWarn("邀請加入車隊結果", $"Result: 無車隊權限 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Team.TeamAuthorityNotEnough
                    };
                }

                #endregion 驗證資料

                #region 發送【更新邀請加入車隊列表】指令至後端

                TeamInviteJoinRequest request = new TeamInviteJoinRequest()
                {
                    MemberID = content.MemberID,
                    Action = (int)ActionType.Add,
                    TeamID = content.TeamID
                };

                CommandData<TeamInviteJoinResponse> response = await this.serverService.DoAction<TeamInviteJoinResponse>((int)TeamCommandIDType.UpdateInviteJoinList, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("邀請加入車隊結果", $"Result: {response.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateInviteJoinListResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)UpdateInviteJoinListResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
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

                #endregion 發送【更新邀請加入車隊列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("邀請加入車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 回覆申請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ResponseApplyJoinTeam(string memberID, TeamResponseApplyJoinContent content)
        {
            try
            {
                #region 驗證資料

                TeamResponseApplyJoinContentValidator teamResponseApplyJoinContentValidator = new TeamResponseApplyJoinContentValidator();
                ValidationResult validationResult = teamResponseApplyJoinContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("回覆申請加入車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (this.MemberHasJoinTeam(content.MemberID, teamDao))
                {
                    this.logger.LogWarn("回覆申請加入車隊結果", $"Result: 會員已加入車隊 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                    };
                }

                if (!teamDao.ApplyJoinList.Contains(content.MemberID))
                {
                    this.logger.LogWarn("回覆申請加入車隊結果", $"Result: 會員未申請加入車隊 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                    };
                }

                if (!this.MemberHasTeamAuthority(memberID, teamDao, false))
                {
                    this.logger.LogWarn("回覆申請加入車隊結果", $"Result: 無車隊權限 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Team.TeamAuthorityNotEnough
                    };
                }

                #endregion 驗證資料

                #region 發送【更新申請加入車隊列表】、【更新隊員列表】指令至後端

                TeamApplyJoinRequest teamApplyJoinRequest = new TeamApplyJoinRequest()
                {
                    MemberID = content.MemberID,
                    Action = (int)ActionType.Delete,
                    TeamID = content.TeamID
                };
                CommandData<TeamApplyJoinResponse> teamApplyJoinResponse = await this.serverService.DoAction<TeamApplyJoinResponse>((int)TeamCommandIDType.UpdateApplyJoinList, CommandType.Team, teamApplyJoinRequest).ConfigureAwait(false);
                this.logger.LogInfo("回覆申請加入車隊結果(更新申請加入車隊列表)", $"Result: {teamApplyJoinResponse.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (teamApplyJoinResponse.Data.Result)
                {
                    case (int)UpdateApplyJoinListResultType.Success:
                        if (content.ResponseType.Equals((int)ActionType.Add))
                        {
                            TeamUpdateMemberRequest teamUpdateMemberRequest = new TeamUpdateMemberRequest()
                            {
                                MemberID = content.MemberID,
                                Action = (int)ActionType.Add,
                                TeamID = content.TeamID
                            };
                            CommandData<TeamUpdateMemberResponse> teamUpdateMemberResponse = await this.serverService.DoAction<TeamUpdateMemberResponse>((int)TeamCommandIDType.UpdateTeamMemberList, CommandType.Team, teamUpdateMemberRequest).ConfigureAwait(false);
                            this.logger.LogInfo("回覆申請加入車隊結果(更新隊員列表)", $"Result: {teamUpdateMemberResponse.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                            switch (teamUpdateMemberResponse.Data.Result)
                            {
                                case (int)UpdateTeamMemberListResultType.Success:
                                    return new ResponseResult()
                                    {
                                        Result = true,
                                        ResultCode = (int)ResponseResultType.Success,
                                        Content = MessageHelper.Message.ResponseMessage.Update.Success
                                    };

                                case (int)UpdateTeamMemberListResultType.Fail:
                                    return new ResponseResult()
                                    {
                                        Result = false,
                                        ResultCode = (int)ResponseResultType.UpdateFail,
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
                        }
                        else
                        {
                            return new ResponseResult()
                            {
                                Result = true,
                                ResultCode = (int)ResponseResultType.Success,
                                Content = MessageHelper.Message.ResponseMessage.Update.Success
                            };
                        }

                    case (int)UpdateApplyJoinListResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
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

                #endregion 發送【更新申請加入車隊列表】、【更新隊員列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("回覆申請加入車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 回覆邀請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ResponseInviteJoinTeam(string memberID, TeamResponseInviteJoinContent content)
        {
            try
            {
                #region 驗證資料

                TeamResponseInviteJoinContentValidator teamResponseInviteJoinContentValidator = new TeamResponseInviteJoinContentValidator();
                ValidationResult validationResult = teamResponseInviteJoinContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("回覆邀請加入車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (this.MemberHasJoinTeam(memberID, teamDao))
                {
                    this.logger.LogWarn("回覆邀請加入車隊結果", $"Result: 會員已加入車隊 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                    };
                }

                if (!teamDao.InviteJoinList.Contains(memberID))
                {
                    this.logger.LogWarn("回覆邀請加入車隊結果", $"Result: 會員未被邀請加入車隊 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Update.Fail
                    };
                }

                #endregion 驗證資料

                #region 發送【更新邀請加入車隊列表】、【更新隊員列表】指令至後端

                TeamInviteJoinRequest teamInviteJoinRequest = new TeamInviteJoinRequest()
                {
                    MemberID = memberID,
                    Action = (int)ActionType.Delete,
                    TeamID = content.TeamID
                };

                CommandData<TeamInviteJoinResponse> response = await this.serverService.DoAction<TeamInviteJoinResponse>((int)TeamCommandIDType.UpdateInviteJoinList, CommandType.Team, teamInviteJoinRequest).ConfigureAwait(false);
                this.logger.LogInfo("回覆邀請加入車隊結果(更新邀請加入車隊列表)", $"Result: {response.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateInviteJoinListResultType.Success:
                        if (content.ResponseType.Equals((int)ActionType.Add))
                        {
                            TeamUpdateMemberRequest teamUpdateMemberRequest = new TeamUpdateMemberRequest()
                            {
                                MemberID = memberID,
                                Action = (int)ActionType.Add,
                                TeamID = content.TeamID
                            };
                            CommandData<TeamUpdateMemberResponse> teamUpdateMemberResponse = await this.serverService.DoAction<TeamUpdateMemberResponse>((int)TeamCommandIDType.UpdateTeamMemberList, CommandType.Team, teamUpdateMemberRequest).ConfigureAwait(false);
                            this.logger.LogInfo("回覆邀請加入車隊結果(更新隊員列表)", $"Result: {teamUpdateMemberResponse.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                            switch (teamUpdateMemberResponse.Data.Result)
                            {
                                case (int)UpdateTeamMemberListResultType.Success:
                                    return new ResponseResult()
                                    {
                                        Result = true,
                                        ResultCode = (int)ResponseResultType.Success,
                                        Content = MessageHelper.Message.ResponseMessage.Update.Success
                                    };

                                case (int)UpdateTeamMemberListResultType.Fail:
                                    return new ResponseResult()
                                    {
                                        Result = false,
                                        ResultCode = (int)ResponseResultType.UpdateFail,
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
                        }
                        else
                        {
                            return new ResponseResult()
                            {
                                Result = true,
                                ResultCode = (int)ResponseResultType.Success,
                                Content = MessageHelper.Message.ResponseMessage.Update.Success
                            };
                        }

                    case (int)UpdateInviteJoinListResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
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

                #endregion 發送【更新邀請加入車隊列表】、【更新隊員列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("回覆邀請加入車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
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