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
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Service.Interfaces.Server;
using DataInfo.Service.Interfaces.Team;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
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
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

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
        /// <param name="mapper">mapper</param>
        /// <param name="serverService">serverService</param>
        /// <param name="teamRepository">teamRepository</param>
        public TeamInteractiveService(IMapper mapper, IServerService serverService, ITeamRepository teamRepository)
        {
            this.mapper = mapper;
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
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ApplyJoinTeam(TeamApplyJoinContent content, string memberID)
        {
            try
            {
                #region 驗證資料

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (teamDao.ApplyJoinList.Contains(memberID))
                {
                    this.logger.LogWarn("申請加入車隊失敗，會員已申請加入車隊，直接回應成功", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = StatusCodes.Status200OK,
                        ResultMessage = ResponseSuccessMessageType.ApplySuccess.ToString()
                    };
                }

                if (this.MemberHasJoinTeam(memberID, teamDao))
                {
                    this.logger.LogWarn("申請加入車隊失敗，會員已加入車隊", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.ApplyFail.ToString()
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
                this.logger.LogInfo("申請加入車隊結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateApplyJoinListResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.ApplySuccess.ToString()
                        };

                    case (int)UpdateApplyJoinListResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.ApplyFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
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
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取消申請加入車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> CancelApplyJoinTeam(TeamApplyJoinContent content, string memberID)
        {
            try
            {
                #region 驗證資料

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (!teamDao.ApplyJoinList.Contains(memberID))
                {
                    this.logger.LogWarn("取消申請加入車隊失敗，會員未申請加入車隊，直接回應成功", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = StatusCodes.Status200OK,
                        ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
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
                this.logger.LogInfo("取消申請加入車隊結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateApplyJoinListResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)UpdateApplyJoinListResultType.Fail:
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

                #endregion 發送【更新申請加入車隊列表】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("取消申請加入車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 回覆申請加入車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ResponseApplyJoinTeam(TeamResponseApplyJoinContent content, string memberID)
        {
            try
            {
                #region 驗證資料

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (!teamDao.ApplyJoinList.Contains(content.MemberID))
                {
                    this.logger.LogWarn("回覆申請加入車隊失敗，會員未申請加入車隊", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.ReplyFail.ToString()
                    };
                }

                if (!this.MemberHasTeamAuthority(memberID, teamDao, false))
                {
                    this.logger.LogWarn("回覆申請加入車隊失敗，無車隊權限", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.TeamAuthorityNotEnough.ToString()
                    };
                }

                if (content.ResponseType.Equals((int)ActionType.Add))
                {
                    if (this.MemberHasJoinTeam(content.MemberID, teamDao))
                    {
                        this.logger.LogWarn("回覆申請加入車隊失敗，會員已加入車隊，直接回應成功", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.ReplySuccess.ToString()
                        };
                    }
                }

                #endregion 驗證資料

                if (content.ResponseType.Equals((int)ActionType.Add))
                {
                    #region 發送【加入或離開車隊】指令至後端

                    TeamJoinOrLeaveRequest teamJoinOrLeaveRequest = this.mapper.Map<TeamJoinOrLeaveRequest>(content);
                    CommandData<TeamJoinOrLeaveResponse> teamJoinOrLeaveResponse = await this.serverService.DoAction<TeamJoinOrLeaveResponse>((int)TeamCommandIDType.JoinOrLeaveTeam, CommandType.Team, teamJoinOrLeaveRequest).ConfigureAwait(false);
                    this.logger.LogInfo("回覆申請加入車隊結果(加入或離開車隊)", $"Response: {JsonConvert.SerializeObject(teamJoinOrLeaveResponse)} Request: {JsonConvert.SerializeObject(teamJoinOrLeaveRequest)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    switch (teamJoinOrLeaveResponse.Data.Result)
                    {
                        case (int)JoinOrLeaveTeamResultType.Success:
                            return new ResponseResult()
                            {
                                Result = true,
                                ResultCode = StatusCodes.Status200OK,
                                ResultMessage = ResponseSuccessMessageType.ReplySuccess.ToString()
                            };

                        case (int)JoinOrLeaveTeamResultType.Fail:
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

                    #endregion 發送【加入或離開車隊】指令至後端
                }
                else if (content.ResponseType.Equals((int)ActionType.Delete))
                {
                    #region 發送【更新申請加入車隊列表】指令至後端

                    TeamApplyJoinRequest teamApplyJoinRequest = new TeamApplyJoinRequest()
                    {
                        MemberID = content.MemberID,
                        Action = (int)ActionType.Delete,
                        TeamID = content.TeamID
                    };
                    CommandData<TeamApplyJoinResponse> teamApplyJoinResponse = await this.serverService.DoAction<TeamApplyJoinResponse>((int)TeamCommandIDType.UpdateApplyJoinList, CommandType.Team, teamApplyJoinRequest).ConfigureAwait(false);
                    this.logger.LogInfo("回覆申請加入車隊結果(更新申請加入車隊列表)", $"Response: {JsonConvert.SerializeObject(teamApplyJoinResponse)} Request: {JsonConvert.SerializeObject(teamApplyJoinRequest)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    switch (teamApplyJoinResponse.Data.Result)
                    {
                        case (int)UpdateApplyJoinListResultType.Success:
                            return new ResponseResult()
                            {
                                Result = true,
                                ResultCode = StatusCodes.Status200OK,
                                ResultMessage = ResponseSuccessMessageType.ReplySuccess.ToString()
                            };

                        case (int)UpdateApplyJoinListResultType.Fail:
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

                    #endregion 發送【更新申請加入車隊列表】指令至後端
                }
                else
                {
                    this.logger.LogWarn("回覆申請加入車隊失敗，回覆動作無效", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.ReplyFail.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("回覆申請加入車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
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