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
    /// 車隊活動服務
    /// </summary>
    public class TeamActivityService : ITeamActivityService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamActivityService");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// teamActivityRepository
        /// </summary>
        private readonly ITeamActivityRepository teamActivityRepository;

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
        /// <param name="teamActivityRepository">teamActivityRepository</param>
        public TeamActivityService(IMapper mapper, IUploadService uploadService, IServerService serverService, ITeamActivityRepository teamActivityRepository)
        {
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.serverService = serverService;
            this.teamActivityRepository = teamActivityRepository;
        }

        /// <summary>
        /// 新增車隊活動
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> AddActivity(string memberID, TeamAddActivityContent content)
        {
            try
            {
                #region 驗證資料

                TeamAddActivityContentValidator teamAddActivityContentValidator = new TeamAddActivityContentValidator();
                ValidationResult validationResult = teamAddActivityContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("新增車隊活動結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 上傳圖片

                List<string> imgBase64s = new List<string>() { content.Photo };
                IEnumerable<string> imgUris = await this.uploadService.UploadTeamActivityImages(imgBase64s, true).ConfigureAwait(false);
                if (imgUris == null || !imgUris.Any())
                {
                    this.logger.LogWarn("新增車隊活動結果", $"Result: 上傳圖片失敗 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = MessageHelper.Message.ResponseMessage.Upload.PhotoFail
                    };
                }

                content.Photo = imgUris.ElementAtOrDefault(0);
                if (string.IsNullOrEmpty(content.Photo))
                {
                    this.logger.LogWarn("新增車隊活動結果", $"Result: 活動圖片轉換失敗 MemberID: {memberID} Photo: {content.Photo}", null);
                }

                #endregion 上傳圖片

                #region 發送【更新活動】指令至後端

                TeamUpdateActivityRequest request = this.mapper.Map<TeamUpdateActivityRequest>(content);
                request.MemberID = memberID;
                request.MemberList = JsonConvert.SerializeObject(new string[] { memberID });
                request.Action = (int)TeamUpdateActivityType.Add;

                CommandData<TeamUpdateActivityResponse> response = await this.serverService.DoAction<TeamUpdateActivityResponse>((int)TeamCommandIDType.UpdateActivity, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("新增車隊活動結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateActivityResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Add.Success
                        };

                    case (int)UpdateActivityResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.CreateFail,
                            Content = MessageHelper.Message.ResponseMessage.Add.Fail
                        };

                    case (int)UpdateActivityResultType.AuthorityNotEnough:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.DenyAccess,
                            Content = MessageHelper.Message.ResponseMessage.Team.TeamAuthorityNotEnough
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Add.Fail
                        };
                }

                #endregion 發送【更新活動】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("新增車隊活動發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Add.Error
                };
            }
        }

        /// <summary>
        /// 取得車隊活動明細資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetTeamActivityDetail(string memberID, TeamActivityDetailContent content)
        {
            try
            {
                #region 驗證資料

                TeamActivityDetailContentValidator teamActivityDetailContentValidator = new TeamActivityDetailContentValidator();
                ValidationResult validationResult = teamActivityDetailContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("取得車隊活動明細資料結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 取得資料

                TeamActivityDao teamActivityDao = await this.teamActivityRepository.Get(memberID, content.TeamID, content.ActID).ConfigureAwait(false);
                if (teamActivityDao == null)
                {
                    this.logger.LogWarn("取得車隊活動明細資料結果", $"Result: 無車隊活動資料 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = MessageHelper.Message.ResponseMessage.Team.ActivityNotExist
                    };
                }

                TeamActivityDetailView teamActivityDetailView = this.mapper.Map<TeamActivityDetailView>(teamActivityDao);
                teamActivityDetailView.HasJoin = teamActivityDao.MemberList.Contains(memberID) ? (int)JoinStatusType.Join : (int)JoinStatusType.None;
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = teamActivityDetailView
                };

                #endregion 取得資料
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊活動明細資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 取得車隊活動列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetTeamActivityList(string memberID, TeamActivityContent content)
        {
            try
            {
                #region 驗證資料

                TeamActivityContentValidator teamActivityContentValidator = new TeamActivityContentValidator();
                ValidationResult validationResult = teamActivityContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("取得車隊活動列表結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 取得資料

                IEnumerable<TeamActivityDao> teamActivityDaos = await this.teamActivityRepository.Get(memberID, content.TeamID).ConfigureAwait(false);
                IEnumerable<TeamActivityListView> teamActivityListViews = teamActivityDaos.Select(dao =>
                {
                    TeamActivityListView teamActivityListView = this.mapper.Map<TeamActivityListView>(dao);
                    teamActivityListView.HasJoin = dao.MemberList.Contains(memberID) ? (int)JoinStatusType.Join : (int)JoinStatusType.None;
                    return teamActivityListView;
                });
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = teamActivityListViews
                };

                #endregion 取得資料
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊活動列表發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }
    }
}