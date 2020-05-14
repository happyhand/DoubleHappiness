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
    /// 車隊服務
    /// </summary>
    public class TeamService : ITeamService
    {
        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// teamRepository
        /// </summary>
        private readonly ITeamRepository teamRepository;

        /// <summary>
        /// logger
        /// </summary>
        protected readonly ILogger logger = LogManager.GetLogger("TeamService");

        /// <summary>
        /// mapper
        /// </summary>
        protected readonly IMapper mapper;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="serverService">serverService</param>
        /// <param name="teamRepository">teamRepository</param>
        public TeamService(IMapper mapper, IServerService serverService, ITeamRepository teamRepository)
        {
            this.mapper = mapper;
            this.serverService = serverService;
            this.teamRepository = teamRepository;
        }

        #region 車隊資料

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

                TeamDao teamDao = (await this.teamRepository.Get(content.TeamName, TeamSearchType.TeamName, false).ConfigureAwait(false)).FirstOrDefault();
                if (teamDao != null)
                {
                    this.logger.LogWarn("建立車隊結果", $"Result: 車隊名稱重複 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.CreateFail,
                        Content = MessageHelper.Message.ResponseMessage.Team.TeamNameRepeat
                    };
                }

                #endregion 驗證資料

                #region 發送【建立車隊】指令至後端

                TeamCreateRequest request = new TeamCreateRequest();

                CommandData<TeamCreateResponse> response = await this.serverService.DoAction<TeamCreateResponse>((int)TeamCommandIDType.CreateNewTeam, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("建立車隊結果", $"Result: {response.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
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

        ///// <summary>
        ///// 建立車隊
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <returns>string</returns>
        //public async Task<string> CreateTeam(TeamDto teamDto)
        //{
        //    try
        //    {
        //        string verifyCreateTeamResult = await this.VerifyCreateTeam(teamDto);
        //        if (!string.IsNullOrEmpty(verifyCreateTeamResult))
        //        {
        //            return verifyCreateTeamResult;
        //        }

        // TeamData teamData = this.CreateTeamData(teamDto); bool isSuccess = await
        // this.teamRepository.CreateTeamData(teamData); if (!isSuccess) { return "建立車隊失敗."; }

        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Create Team Error >>> Data:{JsonConvert.SerializeObject(teamDto)}\n{ex}");
        //        return "建立車隊發生錯誤.";
        //    }
        //}

        ///// <summary>
        ///// 解散車隊
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <returns>string</returns>
        //public async Task<string> DisbandTeam(TeamDto teamDto)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(teamDto.TeamID))
        //        {
        //            return "車隊編號無效.";
        //        }

        // if (string.IsNullOrEmpty(teamDto.ExecutorID)) { return "無法進行解散車隊審核."; }

        // TeamData teamData = await this.teamRepository.GetTeamData(teamDto.TeamID); if (teamData
        // == null) { return "車隊不存在."; }

        // if (!teamData.TeamLeaderID.Equals(teamDto.ExecutorID)) { return "非車隊隊長無法解散車隊."; }

        // bool deleteTeamDataResult = await this.teamRepository.DeleteTeamData(teamData.TeamID); if
        // (!deleteTeamDataResult) { return "解散車隊失敗."; }

        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Disband Team Error >>> TeamID:{teamDto.TeamID} ExecutorID:{teamDto.ExecutorID}\n{ex}");
        //        return "解散車隊發生錯誤.";
        //    }
        //}

        ///// <summary>
        ///// 編輯車隊資料
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <returns>string</returns>
        //public async Task<string> EditTeamData(TeamDto teamDto)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(teamDto.TeamID))
        //        {
        //            return "車隊編號無效.";
        //        }

        // if (string.IsNullOrEmpty(teamDto.ExecutorID)) { return "無法進行編輯車隊資料審核."; }

        // TeamData teamData = await this.teamRepository.GetTeamData(teamDto.TeamID); if (teamData
        // == null) { return "車隊不存在."; }

        // if (!teamData.TeamLeaderID.Equals(teamDto.ExecutorID) &&
        // !teamData.TeamViceLeaderIDs.Contains(teamDto.ExecutorID)) { return "無編輯車隊資料權限."; }

        // this.UpdateTeamDataHandler(teamDto, teamData); bool updateTeamDataResult = await
        // this.teamRepository.UpdateTeamData(teamData); if (!updateTeamDataResult) { return
        // "車隊資料更新失敗."; }

        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Edit Team Data Error >>> Data:{JsonConvert.SerializeObject(teamDto)}\n{ex}");
        //        return "編輯車隊資料發生錯誤.";
        //    }
        //}

        ///// <summary>
        ///// 取得附近車隊資料列表
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <returns>Tuple(TeamDtos, string)</returns>
        //public async Task<Tuple<IEnumerable<TeamDto>, string>> GetNearbyTeamDataList(TeamDto teamDto)
        //{
        //    try
        //    {
        //        if (teamDto.CityID == (int)CityType.None)
        //        {
        //            return Tuple.Create<IEnumerable<TeamDto>, string>(null, "無法查詢該市區附近車隊.");
        //        }

        //        int searchOpenStatus = (int)TeamSearchStatusType.Open;
        //        IEnumerable<TeamData> teamDatas = await this.teamRepository.GetTeamDataListByCityID(teamDto.CityID);
        //        IEnumerable<TeamData> allowTeamDatas = teamDatas.Where(data => data.SearchStatus == searchOpenStatus).Take(10);
        //        return Tuple.Create(this.mapper.Map<IEnumerable<TeamDto>>(allowTeamDatas), string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Get Nearby Team Data List Error >>> CityID:{teamDto.CityID}\n{ex}");
        //        return Tuple.Create<IEnumerable<TeamDto>, string>(null, "取得附近車隊列表發生錯誤.");
        //    }
        //}

        ///// <summary>
        ///// 取得新創車隊資料列表
        ///// </summary>
        ///// <returns>Tuple(TeamDtos, string)</returns>
        //public async Task<Tuple<IEnumerable<TeamDto>, string>> GetNewCreationTeamDataList()
        //{
        //    try
        //    {
        //        //// 時間定義待確認
        //        TimeSpan timeSpan = new TimeSpan(AppSettingHelper.Appsetting.DaysOfNewCreation, 0, 0, 0, 0);
        //        int searchOpenStatus = (int)TeamSearchStatusType.Open;
        //        IEnumerable<TeamData> teamDatas = await this.teamRepository.GetTeamDataListByTimeLimit(timeSpan);
        //        IEnumerable<TeamData> allowTeamDatas = teamDatas.Where(data => data.SearchStatus == searchOpenStatus).Take(10);
        //        return Tuple.Create(this.mapper.Map<IEnumerable<TeamDto>>(allowTeamDatas), string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Get New Creation Team Data List Error\n{ex}");
        //        return Tuple.Create<IEnumerable<TeamDto>, string>(null, "取得新創車隊列表發生錯誤.");
        //    }
        //}

        ///// <summary>
        ///// 取得推薦車隊資料列表
        ///// </summary>
        ///// <returns>Tuple(TeamDtos, string)</returns>
        //public async Task<Tuple<IEnumerable<TeamDto>, string>> GetRecommendationTeamDataList()
        //{
        //    try
        //    {
        //        //// TODO
        //        IEnumerable<TeamDto> teams = new List<TeamDto>();
        //        return Tuple.Create(teams, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Get Recommendation Team Data List Error\n{ex}");
        //        return Tuple.Create<IEnumerable<TeamDto>, string>(null, "取得推薦車隊資料列表發生錯誤.");
        //    }
        //}

        ///// <summary>
        ///// 取得車隊資料
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <returns>Tuple(TeamDto, string)</returns>
        //public async Task<Tuple<TeamDto, string>> GetTeamData(TeamDto teamDto)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(teamDto.TeamID))
        //        {
        //            return Tuple.Create<TeamDto, string>(null, "車隊編號無效.");
        //        }

        // TeamData teamData = await this.teamRepository.GetTeamData(teamDto.TeamID); TeamDto
        // targetTeamDto = this.mapper.Map<TeamDto>(teamData); TeamInteractiveData
        // teamInteractiveData = await
        // this.teamRepository.GetAppointTeamInteractiveData(teamData.TeamID, teamDto.ExecutorID);
        // if (teamInteractiveData != null) { targetTeamDto.JoinStatus =
        // teamInteractiveData.InteractiveType == (int)TeamInteractiveType.Invite ?
        // teamInteractiveData.ReviewFlag == (int)TeamReviewStatusType.Review ?
        // (int)TeamJoinStatusType.WaitInviteExamined : (int)TeamJoinStatusType.BeInvited :
        // (int)TeamJoinStatusType.ApplyFor; } else { targetTeamDto.JoinStatus =
        // teamData.TeamMemberIDs.Contains(teamDto.ExecutorID) ? (int)TeamJoinStatusType.Join :
        // (int)TeamJoinStatusType.None; }

        //        return Tuple.Create(targetTeamDto, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Get Team Data Error >>> TeamID:{teamDto.TeamID}\n{ex}");
        //        return Tuple.Create<TeamDto, string>(null, "取得車隊資料發生錯誤.");
        //    }
        //}

        ///// <summary>
        ///// 取得會員的車隊資料列表
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <returns>Tuple(TeamDtos Of List , string)</returns>
        //public async Task<Tuple<IEnumerable<IEnumerable<TeamDto>>, string>> GetTeamDataListOfMember(TeamDto teamDto)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(teamDto.ExecutorID))
        //        {
        //            return Tuple.Create<IEnumerable<IEnumerable<TeamDto>>, string>(null, "會員編號無效.");
        //        }

        //        IEnumerable<TeamData> teamDatas = await this.teamRepository.GetTeamDataListOfMember(teamDto.ExecutorID);
        //        IEnumerable<TeamData> teamLeaderDatas = teamDatas.Where(data => data.TeamLeaderID.Equals(teamDto.ExecutorID));
        //        IEnumerable<TeamData> teamMemberDatas = teamDatas.Except(teamLeaderDatas);
        //        IEnumerable<TeamInteractiveData> teamInteractiveDatas = await this.teamRepository.GetTeamInteractiveDataListOfMember(teamDto.ExecutorID);
        //        IEnumerable<string> inviteTeamIDs = teamInteractiveDatas.Select(data => data.TeamID).Distinct();
        //        IEnumerable<TeamData> invietTeamDatas = await this.teamRepository.GetTeamDataListByTeamID(inviteTeamIDs);
        //        IEnumerable<IEnumerable<TeamDto>> teamDtos = new List<IEnumerable<TeamDto>>()
        //        {
        //            this.mapper.Map<IEnumerable<TeamDto>>(teamLeaderDatas),
        //            this.mapper.Map<IEnumerable<TeamDto>>(teamMemberDatas),
        //            this.mapper.Map<IEnumerable<TeamDto>>(invietTeamDatas)
        //        };
        //        return Tuple.Create(teamDtos, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Get Team Data List Of Member Error >>> ExecutorID:{teamDto.ExecutorID}\n{ex}");
        //        return Tuple.Create<IEnumerable<IEnumerable<TeamDto>>, string>(null, "取得會員的車隊資料列表發生錯誤.");
        //    }
        //}

        ///// <summary>
        ///// 搜尋車隊
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <returns>Tuple(TeamDtos, string)</returns>
        //public async Task<Tuple<IEnumerable<TeamDto>, string>> SearchTeam(TeamDto teamDto)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(teamDto.SearchKey))
        //        {
        //            return Tuple.Create<IEnumerable<TeamDto>, string>(null, "搜尋關鍵字無效.");
        //        }

        //        int searchOpenStatus = (int)TeamSearchStatusType.Open;
        //        IEnumerable<TeamData> teamDatas = await this.teamRepository.GetTeamDataListByTeamName(teamDto.SearchKey, false);
        //        IEnumerable<TeamData> allowTeamDatas = teamDatas.Where(data => data.SearchStatus == searchOpenStatus);
        //        return Tuple.Create(this.mapper.Map<IEnumerable<TeamDto>>(allowTeamDatas), string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError($"Search Team Error >>> SearchKey:{teamDto.SearchKey}\n{ex}");
        //        return Tuple.Create<IEnumerable<TeamDto>, string>(null, "搜尋車隊發生錯誤.");
        //    }
        //}

        ///// <summary>
        ///// 建立車隊資料
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <returns>TeamData</returns>
        //private TeamData CreateTeamData(TeamDto teamDto)
        //{
        //    DateTime createDate = DateTime.Now;
        //    TeamData teamData = new TeamData()
        //    {
        //        CreateDate = createDate,
        //        TeamID = Utility.GetSerialID(createDate),
        //        TeamName = teamDto.TeamName,
        //        CityID = teamDto.CityID,
        //        TeamInfo = teamDto.TeamInfo,
        //        SearchStatus = teamDto.SearchStatus,
        //        ExamineStatus = teamDto.ExamineStatus,
        //        FrontCoverUrl = teamDto.FrontCoverUrl,
        //        PhotoUrl = teamDto.PhotoUrl,
        //        TeamLeaderID = teamDto.ExecutorID,
        //        TeamViceLeaderIDs = new List<string>(),
        //        TeamMemberIDs = new List<string>() { teamDto.ExecutorID }
        //    };

        //    return teamData;
        //}

        ///// <summary>
        ///// 車隊資料更新處理
        ///// </summary>
        ///// <param name="teamDto">teamDto</param>
        ///// <param name="teamData">teamData</param>
        //private void UpdateTeamDataHandler(TeamDto teamDto, TeamData teamData)
        //{
        //    //// 不修改車隊編號、車隊名稱、車隊所在地

        // if (!string.IsNullOrEmpty(teamDto.TeamInfo)) { teamData.TeamInfo = teamDto.TeamInfo; }

        // if (teamDto.SearchStatus != (int)TeamSearchStatusType.None) { teamData.SearchStatus =
        // teamDto.SearchStatus; }

        // if (teamDto.ExamineStatus != (int)TeamExamineStatusType.None) { teamData.ExamineStatus =
        // teamDto.ExamineStatus; }

        // if (!string.IsNullOrEmpty(teamDto.FrontCoverUrl)) { teamData.FrontCoverUrl =
        // teamDto.FrontCoverUrl; }

        //    if (!string.IsNullOrEmpty(teamDto.PhotoUrl))
        //    {
        //        teamData.PhotoUrl = teamDto.PhotoUrl;
        //    }
        //}

        ///// <summary>
        ///// 驗證車隊建立資料
        ///// </summary>
        ///// <param name="memberDto">memberDto</param>
        ///// <param name="isVerifyPassword">isVerifyPassword</param>
        ///// <returns>string</returns>
        //private async Task<string> VerifyCreateTeam(TeamDto teamDto)
        //{
        //    if (string.IsNullOrEmpty(teamDto.ExecutorID))
        //    {
        //        return "會員編號無效.";
        //    }
        //    else
        //    {
        //        bool isMultipleTeam = await this.teamRepository.VerifyTeamDataByTeamLeaderID(teamDto.ExecutorID);
        //        if (isMultipleTeam)
        //        {
        //            return "無法創建多個車隊.";
        //        }
        //    }

        // if (string.IsNullOrEmpty(teamDto.TeamName)) { return "車隊名稱無效."; } else { bool
        // isRepeatTeamName = await this.teamRepository.VerifyTeamDataByTeamName(teamDto.TeamName);
        // if (isRepeatTeamName) { return "車隊名稱重複."; } }

        // if (teamDto.CityID == (int)CityType.None) { return "未設定車隊所在地."; }

        // if (string.IsNullOrEmpty(teamDto.TeamInfo)) { return "車隊簡介無效."; }

        // if (string.IsNullOrEmpty(teamDto.PhotoUrl)) { return "未上傳車隊頭像."; }

        // if (string.IsNullOrEmpty(teamDto.FrontCoverUrl)) { return "未上傳車隊封面."; }

        // if (teamDto.SearchStatus == (int)TeamSearchStatusType.None) { return "未設定搜尋狀態."; }

        // if (teamDto.ExamineStatus == (int)TeamExamineStatusType.None) { return "未設定審核狀態."; }

        //    return string.Empty;
        //}

        #endregion 車隊資料
    }
}