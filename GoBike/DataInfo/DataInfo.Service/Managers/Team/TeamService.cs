using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dto.Member.View;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Dto.Team.Request;
using DataInfo.Core.Models.Dto.Team.Response;
using DataInfo.Core.Models.Dto.Team.View;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Server;
using DataInfo.Service.Interfaces.Team;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Team
{
    /// <summary>
    /// 車隊服務
    /// </summary>
    public class TeamService : TeamBaseService, ITeamService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamService");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        private readonly IMemberRepository memberRepository;

        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// teamBulletinRepository
        /// </summary>
        private readonly ITeamBulletinRepository teamBulletinRepository;

        /// <summary>
        /// teamRepository
        /// </summary>
        private readonly ITeamRepository teamRepository;

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
        /// <param name="teamRepository">teamRepository</param>
        /// <param name="teamBulletinRepository">teamBulletinRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public TeamService(IMapper mapper, IUploadService uploadService, IServerService serverService, IMemberRepository memberRepository, ITeamRepository teamRepository, ITeamBulletinRepository teamBulletinRepository, IRedisRepository redisRepository) : base(redisRepository)
        {
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.serverService = serverService;
            this.memberRepository = memberRepository;
            this.teamRepository = teamRepository;
            this.teamBulletinRepository = teamBulletinRepository;
        }

        /// <summary>
        /// 刪除車隊快取資訊
        /// </summary>
        /// <param name="teamID">teamID</param>
        private void DeleteTeamCache(string teamID)
        {
            try
            {
                this.logger.LogInfo("刪除車隊快取資訊", $"TeamID: {teamID}", null);
                //// TODO 待測試
                //string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Team}-{teamID}-*";
                //IEnumerable<string> redisKeys = this.redisRepository.GetRedisKeys(AppSettingHelper.Appsetting.Redis.TeamDB, cacheKey);
                //this.redisRepository.DeleteCache(AppSettingHelper.Appsetting.Redis.TeamDB, redisKeys);
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除車隊快取資訊發生錯誤", $"TeamID: {teamID}", ex);
            }
        }

        /// <summary>
        /// 車隊資料更新處理
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>Tuple(string, TeamEditRequest)</returns>
        private async Task<Tuple<string, TeamEditRequest>> UpdateInfoHandler(string memberID, TeamEditContent content)
        {
            if (string.IsNullOrEmpty(content.TeamID))
            {
                return Tuple.Create<string, TeamEditRequest>(ResponseErrorMessageType.TeamIDEmpty.ToString(), null);
            }

            TeamEditRequest request = new TeamEditRequest
            {
                MemberID = memberID,
                TeamID = content.TeamID
            };

            if (!string.IsNullOrEmpty(content.Avatar) || !string.IsNullOrEmpty(content.FrontCover))
            {
                List<string> imgBase64s = new List<string>() { content.Avatar, content.FrontCover };
                IEnumerable<string> imgUris = await this.uploadService.UploadTeamImages(imgBase64s, true).ConfigureAwait(false);
                if (imgUris == null || !imgUris.Any())
                {
                    return Tuple.Create<string, TeamEditRequest>(ResponseErrorMessageType.UploadPhotoFail.ToString(), null);
                }

                if (!string.IsNullOrEmpty(content.Avatar))
                {
                    string avatar = imgUris.ElementAt(0);
                    if (string.IsNullOrEmpty(avatar))
                    {
                        return Tuple.Create<string, TeamEditRequest>(ResponseErrorMessageType.UploadAvatarFail.ToString(), null);
                    }

                    request.Avatar = avatar;
                }

                if (!string.IsNullOrEmpty(content.FrontCover))
                {
                    string frontCover = imgUris.ElementAt(1);
                    if (string.IsNullOrEmpty(frontCover))
                    {
                        return Tuple.Create<string, TeamEditRequest>(ResponseErrorMessageType.UploadFrontCoverFail.ToString(), null);
                    }

                    request.FrontCover = frontCover;
                }
            }

            if (content.ExamineStatus != (int)TeamExamineStatusType.None)
            {
                request.ExamineStatus = content.ExamineStatus;
            }

            if (content.SearchStatus != (int)TeamSearchStatusType.None)
            {
                request.SearchStatus = content.SearchStatus;
            }

            if (!string.IsNullOrEmpty(content.TeamInfo))
            {
                request.TeamInfo = content.TeamInfo;
            }

            return Tuple.Create(string.Empty, request);
        }

        /// <summary>
        /// 更換車隊隊長
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ChangeLeader(TeamChangeLeaderContent content, string memberID)
        {
            try
            {
                #region 發送【更換隊長】指令至後端

                TeamChangeLeaderRequest request = this.mapper.Map<TeamChangeLeaderRequest>(content);
                request.LeaderID = memberID;
                CommandData<TeamChangeLeaderResponse> response = await this.serverService.DoAction<TeamChangeLeaderResponse>((int)TeamCommandIDType.ChangeLeader, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("更換車隊隊長結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);
                switch (response.Data.Result)
                {
                    case (int)ChangeLeaderResultType.Success:
                        this.DeleteTeamCache(content.TeamID);
                        this.UpdateTeamMessageLatestTime(content.TeamID);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.ChangeLeaderSuccess.ToString()
                        };

                    case (int)ChangeLeaderResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.ChangeLeaderFail.ToString()
                        };

                    case (int)ChangeLeaderResultType.Repeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.ChangeLeaderFail.ToString()
                        };

                    case (int)ChangeLeaderResultType.AuthorityNotEnough:
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

                #endregion 發送【更換隊長】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更換車隊隊長發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 建立車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Create(TeamCreateContent content, string memberID)
        {
            try
            {
                #region 上傳圖片

                if (!string.IsNullOrEmpty(content.Avatar) || !string.IsNullOrEmpty(content.FrontCover))
                {
                    List<string> imgBase64s = new List<string>() { content.Avatar, content.FrontCover };
                    IEnumerable<string> imgUris = await this.uploadService.UploadTeamImages(imgBase64s, true).ConfigureAwait(false);
                    if (imgUris == null || !imgUris.Any())
                    {
                        this.logger.LogWarn("建立車隊失敗，上傳圖片失敗", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.UploadPhotoFail.ToString()
                        };
                    }

                    if (!string.IsNullOrEmpty(content.Avatar))
                    {
                        string avatar = imgUris.ElementAt(0);
                        if (string.IsNullOrEmpty(avatar))
                        {
                            this.logger.LogWarn("建立車隊失敗，車隊頭像轉換失敗", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                            return new ResponseResult()
                            {
                                Result = false,
                                ResultCode = StatusCodes.Status400BadRequest,
                                ResultMessage = ResponseErrorMessageType.UploadAvatarFail.ToString()
                            };
                        }

                        content.Avatar = avatar;
                    }

                    if (!string.IsNullOrEmpty(content.FrontCover))
                    {
                        string frontCover = imgUris.ElementAt(1);
                        if (string.IsNullOrEmpty(frontCover))
                        {
                            this.logger.LogWarn("建立車隊失敗，車隊封面轉換失敗", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                            return new ResponseResult()
                            {
                                Result = false,
                                ResultCode = StatusCodes.Status400BadRequest,
                                ResultMessage = ResponseErrorMessageType.UploadFrontCoverFail.ToString()
                            };
                        }

                        content.FrontCover = frontCover;
                    }
                }

                #endregion 上傳圖片

                #region 發送【建立車隊】指令至後端

                TeamCreateRequest request = this.mapper.Map<TeamCreateRequest>(content);
                request.MemberID = memberID;

                CommandData<TeamCreateResponse> response = await this.serverService.DoAction<TeamCreateResponse>((int)TeamCommandIDType.CreateNewTeam, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("建立車隊結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);
                switch (response.Data.Result)
                {
                    case (int)CreateNewTeamResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.CreateSuccess.ToString()
                        };

                    case (int)CreateNewTeamResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.CreateFail.ToString()
                        };

                    case (int)CreateNewTeamResultType.Repeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.RepeatFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【建立車隊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("建立車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 解散車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Disband(TeamContent content, string memberID)
        {
            try
            {
                #region 發送【解散車隊】指令至後端

                TeamDisbandRequest request = this.mapper.Map<TeamDisbandRequest>(content);
                request.MemberID = memberID;

                CommandData<TeamDisbandResponse> response = await this.serverService.DoAction<TeamDisbandResponse>((int)TeamCommandIDType.DeleteTeam, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("解散車隊結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);
                switch (response.Data.Result)
                {
                    case (int)DeleteTeamResultType.Success:
                        this.DeleteTeamCache(content.TeamID);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.DisbandSuccess.ToString()
                        };

                    case (int)DeleteTeamResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.DisbandFail.ToString()
                        };

                    case (int)DeleteTeamResultType.AuthorityNotEnough:
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

                #endregion 發送【解散車隊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("解散車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 更新車隊資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Edit(TeamEditContent content, string memberID)
        {
            try
            {
                #region 處理更新資料

                Tuple<string, TeamEditRequest> updateInfoHandlerResult = await this.UpdateInfoHandler(memberID, content).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(updateInfoHandlerResult.Item1))
                {
                    this.logger.LogWarn("更新車隊資料更新失敗，資料驗證錯誤", $"Result: 更新失敗({updateInfoHandlerResult.Item1}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} Error: {updateInfoHandlerResult.Item1}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = updateInfoHandlerResult.Item1
                    };
                }

                TeamEditRequest request = updateInfoHandlerResult.Item2;

                #endregion 處理更新資料

                #region 發送【更新車隊資訊】指令至後端

                CommandData<TeamEditResponse> response = await this.serverService.DoAction<TeamEditResponse>((int)TeamCommandIDType.UpdateTeamData, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("更新車隊資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateTeamDataResultType.Success:
                        this.DeleteTeamCache(content.TeamID);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)UpdateTeamDataResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    case (int)UpdateTeamDataResultType.AuthorityNotEnough:
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

                #endregion 發送【更新車隊資訊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新車隊資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得瀏覽車隊資訊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetBrowseInfo(TeamBrowseContent content, string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.BrowseInfo}";
                //IEnumerable<IEnumerable<TeamSearchView>> teamBrowseInfoViews = await this.redisRepository.GetCache<IEnumerable<IEnumerable<TeamSearchView>>>(AppSettingHelper.Appsetting.Redis.MemberDB, cacheKey).ConfigureAwait(false);
                IEnumerable<IEnumerable<TeamSearchView>> teamBrowseInfoViews = null;
                if (teamBrowseInfoViews == null)
                {
                    Task<IEnumerable<TeamDao>> nearbyTeamListTask = this.teamRepository.GetNearbyTeam(memberID, content.County);
                    Task<IEnumerable<TeamDao>> newCreationTeamListTask = this.teamRepository.GetNewCreationTeam(memberID);
                    Task<IEnumerable<TeamDao>> recommendTeamListTask = this.teamRepository.GetRecommendTeam(memberID);
                    IEnumerable<TeamDao>[] teamDaos = new IEnumerable<TeamDao>[]
                    {
                    await nearbyTeamListTask.ConfigureAwait(false),
                    await newCreationTeamListTask.ConfigureAwait(false),
                    await recommendTeamListTask.ConfigureAwait(false)
                    };

                    teamBrowseInfoViews = this.mapper.Map<IEnumerable<IEnumerable<TeamSearchView>>>(teamDaos);
                    //this.redisRepository.SetCache(AppSettingHelper.Appsetting.Redis.MemberDB, cacheKey, JsonConvert.SerializeObject(teamBrowseInfoViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = teamBrowseInfoViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得瀏覽車隊資訊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得車隊下拉選單
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetDropMenu(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.DropMenu}";
                //IEnumerable<TeamDropMenuView>[] teamDropMenuViews = await this.redisRepository.GetCache<IEnumerable<TeamDropMenuView>[]>(AppSettingHelper.Appsetting.Redis.MemberDB, cacheKey).ConfigureAwait(false);
                IEnumerable<TeamDropMenuView>[] teamDropMenuViews = null;
                if (teamDropMenuViews == null)
                {
                    MemberDao memberDao = await this.memberRepository.Get(memberID, MemberSearchType.MemberID).ConfigureAwait(false);
                    if (memberDao == null)
                    {
                        this.logger.LogWarn("取得車隊下拉選單失敗，無會員資料", $"MemberID: {memberID}", null);
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.MemberIDEmpty.ToString()
                        };
                    }

                    IEnumerable<TeamDao> joinTeamDaos = await this.teamRepository.Get(memberDao.TeamList).ConfigureAwait(false);
                    joinTeamDaos = joinTeamDaos.OrderByDescending(dao => dao.Leader.Equals(memberID));
                    IEnumerable<TeamDao> applyTeamDaos = await this.teamRepository.GetTeamOfApplyJoin(memberID).ConfigureAwait(false);
                    IEnumerable<TeamDropMenuView> joinTeamDropMenuViews = this.mapper.Map<IEnumerable<TeamDropMenuView>>(joinTeamDaos);
                    IEnumerable<TeamDropMenuView> applyTeamDropMenuViews = this.mapper.Map<IEnumerable<TeamDropMenuView>>(applyTeamDaos);
                    foreach (TeamDropMenuView joinTeamDropMenuView in joinTeamDropMenuViews)
                    {
                        string messageLatestTimeCacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Team}-{joinTeamDropMenuView.TeamID}-{AppSettingHelper.Appsetting.Redis.SubFlag.MessageLatestTime}";
                        DateTime messageLatestTime = await this.redisRepository.GetCache<DateTime>(AppSettingHelper.Appsetting.Redis.TeamDB, messageLatestTimeCacheKey).ConfigureAwait(false);
                        this.logger.LogInfo("取得車隊下拉選單", $"MessageLatestTime: {JsonConvert.SerializeObject(messageLatestTime)}  messageLatestTime:{ messageLatestTime}", null);
                        joinTeamDropMenuView.MessageLatestTime = messageLatestTime;
                    }

                    teamDropMenuViews = new IEnumerable<TeamDropMenuView>[] {
                    joinTeamDropMenuViews,
                    applyTeamDropMenuViews,
                    };

                    //this.redisRepository.SetCache(AppSettingHelper.Appsetting.Redis.MemberDB, cacheKey, JsonConvert.SerializeObject(teamDropMenuViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = teamDropMenuViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊下拉選單發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得車隊資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetInfo(string memberID, string teamID)
        {
            try
            {
                #region 驗證資料

                if (string.IsNullOrEmpty(teamID))
                {
                    this.logger.LogWarn("取得車隊資訊失敗，無車隊 ID", $"MemberID: {memberID} TeamID: {teamID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = ResponseErrorMessageType.TeamIDEmpty.ToString()
                    };
                }

                #endregion 驗證資料

                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Team}-{teamID}-{AppSettingHelper.Appsetting.Redis.SubFlag.TeamInfo}-{memberID}";
                //TeamInfoView teamInfoView = await this.redisRepository.GetCache<TeamInfoView>(AppSettingHelper.Appsetting.Redis.TeamDB, cacheKey).ConfigureAwait(false);
                TeamInfoView teamInfoView = null;
                if (teamInfoView == null)
                {
                    #region 取得車隊資料

                    TeamDao teamDao = await this.teamRepository.Get(teamID).ConfigureAwait(false);
                    if (teamDao == null)
                    {
                        this.logger.LogWarn("取得車隊設定資訊失敗，無車隊資料", $"MemberID: {memberID} TeamID: {teamID}", null);
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.GetFail.ToString()
                        };
                    }

                    #endregion 取得車隊資料

                    #region 取得資料

                    teamInfoView = this.mapper.Map<TeamInfoView>(teamDao);
                    IEnumerable<string> teamMemberIDs = (new string[] { teamDao.Leader }).Concat(teamDao.TeamViceLeaderIDs).Concat(teamDao.TeamMemberIDs);
                    IEnumerable<MemberDao> teamMemberDaos = await this.memberRepository.Get(teamMemberIDs, null).ConfigureAwait(false);
                    TeamRoleType role = this.GetTeamRole(memberID, teamDao);
                    List<TeamMemberView> teamMemberViews = new List<TeamMemberView>();
                    foreach (MemberDao memberDao in teamMemberDaos)
                    {
                        TeamMemberView teamMemberView = new TeamMemberView()
                        {
                            Avatar = memberDao.Avatar,
                            MemberID = memberDao.MemberID,
                            Nickname = string.IsNullOrEmpty(memberDao.Nickname) ? memberDao.MemberID : memberDao.Nickname,
                            Role = (int)this.GetTeamRole(memberDao.MemberID, teamDao)
                        };
                        teamMemberView.OnlineType = role.Equals(TeamRoleType.None) ? (int)OnlineStatusType.None : await this.memberRepository.GetOnlineType(memberDao.MemberID).ConfigureAwait(false);
                        teamMemberViews.Add(teamMemberView);
                    }

                    teamInfoView.MemberList = teamMemberViews.OrderByDescending(view => view.Role).ThenBy(view => view.Nickname);
                    teamInfoView.InteractiveStatus = (int)this.GetTeamInteractiveStatus(memberID, teamDao);
                    teamInfoView.Role = (int)role;
                    //this.redisRepository.SetCache(AppSettingHelper.Appsetting.Redis.TeamDB, cacheKey, JsonConvert.SerializeObject(teamInfoView), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));

                    #endregion 取得資料
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = teamInfoView
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊資訊發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得車隊訊息
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetMessage(TeamContent content, string memberID)
        {
            try
            {
                #region 取得資料

                Task<IEnumerable<TeamBulletinDao>> teamBulletinDaos = this.teamBulletinRepository.Get(memberID, content.TeamID);
                Task<IEnumerable<MemberDao>> memberOfApplyJoinList = this.teamRepository.GetMemberOfApplyJoin(memberID, content.TeamID);
                Task<IEnumerable<MemberDao>> memberOfTeamList = this.teamRepository.GetMemberList(memberID, content.TeamID);
                IEnumerable<TeamBulletinDao> filterTeamBulletinDaos = (await teamBulletinDaos.ConfigureAwait(false)).Where(dao =>
                {
                    DateTime createDate = Convert.ToDateTime(dao.CreateDate);
                    DateTime expirationDate = createDate.AddDays(dao.Day);
                    return expirationDate >= DateTime.UtcNow;
                }).OrderByDescending(dao => Convert.ToDateTime(dao.CreateDate));
                IEnumerable<TeamBullentiListView> teamBullentiListViews = this.mapper.Map<IEnumerable<TeamBullentiListView>>(filterTeamBulletinDaos);
                IEnumerable<MemberSimpleInfoView> applyJoinViews = this.mapper.Map<IEnumerable<MemberSimpleInfoView>>(await memberOfApplyJoinList.ConfigureAwait(false));
                IEnumerable<MemberSimpleInfoView> teamMemberViews = this.mapper.Map<IEnumerable<MemberSimpleInfoView>>(await memberOfTeamList.ConfigureAwait(false));

                string messageLatestTimeCacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Team}-{content.TeamID}-{AppSettingHelper.Appsetting.Redis.SubFlag.MessageLatestTime}";
                DateTime messageLatestTime = await this.redisRepository.GetCache<DateTime>(AppSettingHelper.Appsetting.Redis.TeamDB, messageLatestTimeCacheKey).ConfigureAwait(false);

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = new TeamMessageView()
                    {
                        MessageLatestTime = messageLatestTime,
                        BullentiList = teamBullentiListViews,
                        ApplyJoinList = applyJoinViews,
                        MemberList = teamMemberViews
                    }
                };

                #endregion 取得資料
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊訊息發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得車隊設定資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetSetting(string memberID, string teamID)
        {
            try
            {
                #region 驗證資料

                if (string.IsNullOrEmpty(teamID))
                {
                    this.logger.LogWarn("取得車隊設定資訊失敗，無車隊 ID", $"MemberID: {memberID} TeamID: {teamID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = ResponseErrorMessageType.TeamIDEmpty.ToString()
                    };
                }

                #endregion 驗證資料

                #region 取得車隊資料

                TeamDao teamDao = await this.teamRepository.Get(teamID).ConfigureAwait(false);
                if (teamDao == null)
                {
                    this.logger.LogWarn("取得車隊設定資訊失敗，無車隊資料", $"MemberID: {memberID} TeamID: {teamID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.GetFail.ToString()
                    };
                }

                #endregion 取得車隊資料

                if (!this.MemberHasTeamAuthority(memberID, teamDao, false))
                {
                    this.logger.LogWarn("取得車隊設定資訊失敗，無車隊權限", $"MemberID: {memberID} TeamID: {teamID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.TeamAuthorityNotEnough.ToString()
                    };
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = this.mapper.Map<TeamSettingView>(teamDao)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊設定資訊失敗發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 踢離車隊隊員
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Kick(TeamKickContent content, string memberID)
        {
            try
            {
                #region 發送【踢離車隊成員】指令至後端

                TeamKickRequest request = new TeamKickRequest()
                {
                    MemberID = memberID,
                    TeamID = content.TeamID,
                    KickIdList = JsonConvert.SerializeObject(new string[] { content.MemberID })
                };

                CommandData<TeamKickResponse> response = await this.serverService.DoAction<TeamKickResponse>((int)TeamCommandIDType.KickTeamMember, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("踢離車隊隊員結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);
                switch (response.Data.Result)
                {
                    case (int)KickTeamMemberResultType.Success:
                        this.DeleteTeamCache(content.TeamID);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.KickSuccess.ToString()
                        };

                    case (int)KickTeamMemberResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.KickFail.ToString()
                        };

                    case (int)KickTeamMemberResultType.AuthorityNotEnough:
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

                #endregion 發送【踢離車隊成員】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("踢離車隊隊員發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 離開車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Leave(string memberID, string teamID)
        {
            try
            {
                #region 驗證資料

                if (string.IsNullOrEmpty(teamID))
                {
                    this.logger.LogWarn("離開車隊失敗，無車隊 ID", $"MemberID: {memberID} TeamID: {teamID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = ResponseErrorMessageType.TeamIDEmpty.ToString()
                    };
                }

                #endregion 驗證資料

                #region 發送【加入或離開車隊】指令至後端

                TeamJoinOrLeaveRequest teamJoinOrLeaveRequest = new TeamJoinOrLeaveRequest() { MemberID = memberID, TeamID = teamID, Action = (int)ActionType.Delete };
                CommandData<TeamJoinOrLeaveResponse> teamJoinOrLeaveResponse = await this.serverService.DoAction<TeamJoinOrLeaveResponse>((int)TeamCommandIDType.JoinOrLeaveTeam, CommandType.Team, teamJoinOrLeaveRequest).ConfigureAwait(false);
                this.logger.LogInfo("離開車隊結果", $"Response: {JsonConvert.SerializeObject(teamJoinOrLeaveResponse)} Request: {JsonConvert.SerializeObject(teamJoinOrLeaveRequest)}", null);
                switch (teamJoinOrLeaveResponse.Data.Result)
                {
                    case (int)JoinOrLeaveTeamResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)JoinOrLeaveTeamResultType.Fail:
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

                #endregion 發送【加入或離開車隊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("離開車隊發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 搜尋車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Search(TeamSearchContent content, string memberID)
        {
            try
            {
                IEnumerable<TeamDao> teamDaos = await this.teamRepository.Search(content.SearchKey).ConfigureAwait(false);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = this.mapper.Map<IEnumerable<TeamSearchView>>(teamDaos)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 更新車隊副隊長
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="actionType">actionType</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdateViceLeader(TeamUpdateViceLeaderContent content, string memberID, ActionType action)
        {
            try
            {
                #region 發送【更新副隊長】指令至後端

                TeamUpdateViceLeaderRequest request = this.mapper.Map<TeamUpdateViceLeaderRequest>(content);
                request.LeaderID = memberID;
                request.Action = (int)action;
                CommandData<TeamUpdateViceLeaderResponse> response = await this.serverService.DoAction<TeamUpdateViceLeaderResponse>((int)TeamCommandIDType.UpdateViceLeaderList, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("更新車隊副隊長結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)ChangeLeaderResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)ChangeLeaderResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    case (int)ChangeLeaderResultType.AuthorityNotEnough:
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

                #endregion 發送【更新副隊長】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新車隊副隊長發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
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