using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Dto.Team.Request;
using DataInfo.Core.Models.Dto.Team.Response;
using DataInfo.Core.Models.Dto.Team.View;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Server;
using DataInfo.Service.Interfaces.Team;
using FluentValidation.Results;
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
    public class TeamService : ITeamService
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
        public TeamService(IMapper mapper, IUploadService uploadService, IServerService serverService, IMemberRepository memberRepository, ITeamRepository teamRepository)
        {
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.serverService = serverService;
            this.memberRepository = memberRepository;
            this.teamRepository = teamRepository;
        }

        #region 車隊資料

        /// <summary>
        /// 取得車隊互動狀態
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamDao">teamDao</param>
        /// <returns>TeamInteractiveType</returns>
        private TeamInteractiveType GetTeamInteractiveStatus(string memberID, TeamDao teamDao)
        {
            if (teamDao.ApplyJoinList.Contains(memberID))
            {
                return TeamInteractiveType.ApplyJoin;
            }
            else if (teamDao.InviteJoinList.Contains(memberID))
            {
                return TeamInteractiveType.InviteJoin;
            }
            else if (teamDao.Leader.Equals(memberID) || teamDao.TeamViceLeaderIDs.Contains(memberID) || teamDao.TeamMemberIDs.Contains(memberID))
            {
                return TeamInteractiveType.Member;
            }
            else
            {
                return TeamInteractiveType.None;
            }
        }

        /// <summary>
        /// 取得車隊角色
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamDao">teamDao</param>
        /// <returns>TeamRoleType</returns>
        private TeamRoleType GetTeamRole(string memberID, TeamDao teamDao)
        {
            if (teamDao.Leader.Equals(memberID))
            {
                return TeamRoleType.Leader;
            }
            else if (teamDao.TeamViceLeaderIDs.Contains(memberID))
            {
                return TeamRoleType.ViceLeader;
            }
            else if (teamDao.TeamMemberIDs.Contains(memberID))
            {
                return TeamRoleType.Normal;
            }
            else
            {
                return TeamRoleType.None;
            }
        }

        /// <summary>
        /// 更換車隊隊長
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ChangeLeader(string memberID, TeamChangeLeaderContent content)
        {
            try
            {
                #region 驗證資料

                TeamChangeLeaderContentValidator contentValidator = new TeamChangeLeaderContentValidator();
                ValidationResult validationResult = contentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("更換車隊隊長結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 發送【更換隊長】指令至後端

                TeamChangeLeaderRequest request = this.mapper.Map<TeamChangeLeaderRequest>(content);
                request.LeaderID = memberID;
                CommandData<TeamChangeLeaderResponse> response = await this.serverService.DoAction<TeamChangeLeaderResponse>((int)TeamCommandIDType.ChangeLeader, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("更換車隊隊長結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)ChangeLeaderResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)ChangeLeaderResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    case (int)ChangeLeaderResultType.Repeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.Repeat,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    case (int)ChangeLeaderResultType.AuthorityNotEnough:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.DenyAccess,
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

                #endregion 發送【更換隊長】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更換車隊隊長發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 建立車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Create(string memberID, TeamCreateContent content)
        {
            try
            {
                #region 驗證資料

                TeamCreateContentValidator teamCreateContentValidator = new TeamCreateContentValidator();
                ValidationResult validationResult = teamCreateContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("建立車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 上傳圖片

                List<string> imgBase64s = new List<string>() { content.Avatar, content.FrontCover };
                IEnumerable<string> imgUris = await this.uploadService.UploadTeamImages(imgBase64s, true).ConfigureAwait(false);
                if (imgUris == null || !imgUris.Any())
                {
                    this.logger.LogWarn("建立車隊結果", $"Result: 上傳圖片失敗 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = MessageHelper.Message.ResponseMessage.Upload.PhotoFail
                    };
                }

                content.Avatar = imgUris.ElementAtOrDefault(0);
                content.FrontCover = imgUris.ElementAtOrDefault(1);
                if (string.IsNullOrEmpty(content.Avatar) || string.IsNullOrEmpty(content.FrontCover))
                {
                    this.logger.LogWarn("建立車隊結果", $"Result: 車隊圖片轉換失敗 MemberID: {memberID} Avatar: {content.Avatar} FrontCover: {content.FrontCover}", null);
                }

                #endregion 上傳圖片

                #region 發送【建立車隊】指令至後端

                TeamCreateRequest request = this.mapper.Map<TeamCreateRequest>(content);
                request.MemberID = memberID;

                CommandData<TeamCreateResponse> response = await this.serverService.DoAction<TeamCreateResponse>((int)TeamCommandIDType.CreateNewTeam, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("建立車隊結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)CreateNewTeamResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Add.Success
                        };

                    case (int)CreateNewTeamResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.CreateFail,
                            Content = MessageHelper.Message.ResponseMessage.Add.Fail
                        };

                    case (int)CreateNewTeamResultType.Repeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.Repeat,
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

                #endregion 發送【建立車隊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("建立車隊發生錯誤", $"MemberID: {memberID} IsValidatePassword: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Add.Error
                };
            }
        }

        /// <summary>
        /// 取得瀏覽車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetBrowseTeam(string memberID, TeamBrowseContent content)
        {
            try
            {
                #region 驗證資料

                TeamBrowseContentValidator teamRecommendContentValidator = new TeamBrowseContentValidator();
                ValidationResult validationResult = teamRecommendContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("取得瀏覽車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                Task<IEnumerable<TeamDao>> nearbyTeamListTask = this.teamRepository.GetNearbyTeam(content.County);
                Task<IEnumerable<TeamDao>> newCreationTeamListTask = this.teamRepository.GetNewCreationTeam();
                Task<IEnumerable<TeamDao>> recommendTeamListTask = this.teamRepository.GetRecommendTeam();
                IEnumerable<TeamDao>[] teamDaos = new IEnumerable<TeamDao>[]
                {
                    await nearbyTeamListTask.ConfigureAwait(false),
                    await newCreationTeamListTask.ConfigureAwait(false),
                    await recommendTeamListTask.ConfigureAwait(false)
                };

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = this.mapper.Map<IEnumerable<IEnumerable<TeamSearchView>>>(teamDaos)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得瀏覽車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 取得車隊下拉選單
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetTeamDropMenu(string memberID)
        {
            try
            {
                MemberDao memberDao = await this.memberRepository.Get(memberID).ConfigureAwait(false);
                if (memberDao == null)
                {
                    this.logger.LogWarn("取得車隊下拉選單結果", $"Result: 無會員資料 MemberID: {memberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = MessageHelper.Message.ResponseMessage.Member.MemberNotExist
                    };
                }

                Task<IEnumerable<TeamDao>> joinTeamDaosTask = this.teamRepository.Get(memberDao.TeamList);
                Task<IEnumerable<TeamDao>> applyTeamDaosTask = this.teamRepository.GetApplyTeamList(memberID);
                Task<IEnumerable<TeamDao>> inviteTeamDaosTask = this.teamRepository.GetInviteTeamList(memberID);
                IEnumerable<TeamDropMenuView> joinTeamDropMenuView = this.mapper.Map<IEnumerable<TeamDropMenuView>>(await joinTeamDaosTask.ConfigureAwait(false));
                IEnumerable<TeamDropMenuView> applyTeamDropMenuView = this.mapper.Map<IEnumerable<TeamDropMenuView>>(await applyTeamDaosTask.ConfigureAwait(false));
                IEnumerable<TeamDropMenuView> inviteTeamDropMenuView = this.mapper.Map<IEnumerable<TeamDropMenuView>>(await inviteTeamDaosTask.ConfigureAwait(false));

                //// TODO 已加入的車隊需顯示是否有新的訊息

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = new List<IEnumerable<TeamDropMenuView>>() {
                    joinTeamDropMenuView,
                    applyTeamDropMenuView,
                    inviteTeamDropMenuView
                    }
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊下拉選單發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 取得車隊資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetTeamInfo(string memberID, TeamContent content)
        {
            try
            {
                #region 驗證資料

                TeamContentValidator teamContentValidator = new TeamContentValidator();
                ValidationResult validationResult = teamContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("取得車隊資訊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 取得車隊資料

                TeamDao teamDao = await this.teamRepository.Get(content.TeamID).ConfigureAwait(false);
                if (teamDao == null)
                {
                    this.logger.LogWarn("取得車隊資訊結果", $"Result: 無車隊資料 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = MessageHelper.Message.ResponseMessage.Get.Fail
                    };
                }

                #endregion 取得車隊資料

                #region 取得資料

                TeamInfoView teamInfoView = this.mapper.Map<TeamInfoView>(teamDao);
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
                        Nickname = memberDao.Nickname,
                        Role = (int)this.GetTeamRole(memberDao.MemberID, teamDao)
                    };
                    teamMemberView.OnlineType = role.Equals(TeamRoleType.None) ? (int)OnlineStatusType.None : await this.memberRepository.GetOnlineType(memberDao.MemberID).ConfigureAwait(false);
                    teamMemberViews.Add(teamMemberView);
                }

                teamInfoView.MemberList = teamMemberViews;
                teamInfoView.InteractiveStatus = (int)this.GetTeamInteractiveStatus(memberID, teamDao);

                #endregion 取得資料

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = teamInfoView
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊資訊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 搜尋車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> SearchTeam(string memberID, TeamSearchContent content)
        {
            try
            {
                #region 驗證資料

                TeamSearchContentValidator teamSearchContentValidator = new TeamSearchContentValidator();
                ValidationResult validationResult = teamSearchContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("搜尋車隊結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                IEnumerable<TeamDao> teamDaos = await this.teamRepository.Search(content.SearchKey).ConfigureAwait(false);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = this.mapper.Map<IEnumerable<TeamSearchView>>(teamDaos)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 更新車隊副隊長
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <param name="actionType">actionType</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdateViceLeader(string memberID, TeamUpdateViceLeaderContent content, ActionType action)
        {
            try
            {
                #region 驗證資料

                TeamUpdateViceLeaderContentValidator contentValidator = new TeamUpdateViceLeaderContentValidator();
                ValidationResult validationResult = contentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("更新車隊副隊長結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} Action: {action}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

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
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)ChangeLeaderResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    case (int)ChangeLeaderResultType.AuthorityNotEnough:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.DenyAccess,
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

                #endregion 發送【更新副隊長】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新車隊副隊長發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        #endregion 車隊資料
    }
}