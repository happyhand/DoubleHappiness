using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Dto.Team.Request;
using DataInfo.Core.Models.Dto.Team.Response;
using DataInfo.Core.Models.Dto.Team.View;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Service.Interfaces.Server;
using DataInfo.Service.Interfaces.Team;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Team
{
    /// <summary>
    /// 車隊公告服務
    /// </summary>
    public class TeamBulletinService : TeamBaseService, ITeamBulletinService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamBulletinService");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// teamBulletinRepository
        /// </summary>
        private readonly ITeamBulletinRepository teamBulletinRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="serverService">serverService</param>
        /// <param name="teamBulletinRepository">teamBulletinRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public TeamBulletinService(IMapper mapper, IServerService serverService, ITeamBulletinRepository teamBulletinRepository, IRedisRepository redisRepository) : base(redisRepository)
        {
            this.mapper = mapper;
            this.serverService = serverService;
            this.teamBulletinRepository = teamBulletinRepository;
        }

        /// <summary>
        /// 車隊公告資料更新處理
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>Tuple(string, TeamUpdateBulletinRequest)</returns>
        private Tuple<string, TeamUpdateBulletinRequest> UpdateHandler(string memberID, TeamUpdateBulletinContent content)
        {
            if (string.IsNullOrEmpty(content.BulletinID))
            {
                return Tuple.Create<string, TeamUpdateBulletinRequest>(ResponseErrorMessageType.BulletinIDEmpty.ToString(), null);
            }

            TeamUpdateBulletinRequest request = new TeamUpdateBulletinRequest()
            {
                BulletinID = content.BulletinID,
                MemberID = memberID
            };

            if (!string.IsNullOrEmpty(content.Content))
            {
                request.Content = content.Content;
            }

            request.Action = (int)ActionType.Edit;
            return Tuple.Create(string.Empty, request);
        }

        /// <summary>
        /// 新增車隊公告
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Add(TeamAddBulletinContent content, string memberID)
        {
            try
            {
                #region 發送【更新公告】指令至後端

                TeamUpdateBulletinRequest request = this.mapper.Map<TeamUpdateBulletinRequest>(content);
                request.MemberID = memberID;
                request.Action = (int)ActionType.Add;

                CommandData<TeamUpdateBulletinResponse> response = await this.serverService.DoAction<TeamUpdateBulletinResponse>((int)TeamCommandIDType.UpdateBulletin, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("新增車隊公告結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateBulletinResultType.Success:
                        this.UpdateTeamMessageLatestTime(content.TeamID);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.CreateSuccess.ToString()
                        };

                    case (int)UpdateBulletinResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status500InternalServerError,
                            ResultMessage = ResponseErrorMessageType.CreateFail.ToString()
                        };

                    case (int)UpdateBulletinResultType.AuthorityNotEnough:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.TeamAuthorityNotEnough.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【更新公告】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("新增車隊公告發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 刪除車隊公告
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="bulletinID">bulletinID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Delete(string memberID, string bulletinID)
        {
            try
            {
                #region 驗證資料

                if (string.IsNullOrEmpty(bulletinID))
                {
                    this.logger.LogWarn("刪除車隊公告失敗，無車隊公告資料", $"MemberID: {memberID} BulletinID: {bulletinID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.BulletinIDEmpty.ToString()
                    };
                }

                #endregion 驗證資料

                #region 發送【更新公告】指令至後端

                TeamUpdateBulletinRequest request = new TeamUpdateBulletinRequest()
                {
                    MemberID = memberID,
                    BulletinID = bulletinID,
                    Action = (int)ActionType.Delete
                };

                CommandData<TeamUpdateBulletinResponse> response = await this.serverService.DoAction<TeamUpdateBulletinResponse>((int)TeamCommandIDType.UpdateBulletin, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("刪除車隊公告結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} BulletinID: {bulletinID}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateBulletinResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)UpdateBulletinResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status500InternalServerError,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    case (int)UpdateBulletinResultType.AuthorityNotEnough:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.TeamAuthorityNotEnough.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【更新公告】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除車隊公告發生錯誤", $"MemberID: {memberID} BulletinID: {bulletinID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 更新車隊公告
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Edit(TeamUpdateBulletinContent content, string memberID)
        {
            try
            {
                #region 處理更新資料

                Tuple<string, TeamUpdateBulletinRequest> updateHandlerResult = this.UpdateHandler(memberID, content);
                if (!string.IsNullOrEmpty(updateHandlerResult.Item1))
                {
                    this.logger.LogWarn("更新車隊公告資料失敗，資料驗證錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = updateHandlerResult.Item1
                    };
                }

                TeamUpdateBulletinRequest request = updateHandlerResult.Item2;

                #endregion 處理更新資料

                #region 發送【更新公告】指令至後端

                CommandData<TeamUpdateBulletinResponse> response = await this.serverService.DoAction<TeamUpdateBulletinResponse>((int)TeamCommandIDType.UpdateBulletin, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("更新車隊公告資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateBulletinResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)UpdateBulletinResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status500InternalServerError,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    case (int)UpdateBulletinResultType.AuthorityNotEnough:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.TeamAuthorityNotEnough.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【更新公告】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新車隊公告資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得車隊公告列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetList(string memberID, string teamID)
        {
            try
            {
                #region 驗證資料

                if (string.IsNullOrEmpty(teamID))
                {
                    this.logger.LogWarn("取得車隊公告列表失敗，無車隊資料", $"MemberID: {memberID} TeamID: {teamID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.TeamIDEmpty.ToString()
                    };
                }

                #endregion 驗證資料

                #region 取得資料

                IEnumerable<TeamBulletinDao> teamBulletinDaos = await this.teamBulletinRepository.Get(memberID, teamID).ConfigureAwait(false);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = this.mapper.Map<IEnumerable<TeamBulletinDao>>(teamBulletinDaos)
                };

                #endregion 取得資料
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊公告列表發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
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