﻿using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Dto.Team.Content.data;
using DataInfo.Core.Models.Dto.Team.Request;
using DataInfo.Core.Models.Dto.Team.Response;
using DataInfo.Core.Models.Dto.Team.View;
using DataInfo.Core.Models.Enum;
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
        /// 車隊活動資料更新處理
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>Tuple(string, TeamUpdateActivityRequest)</returns>
        private async Task<Tuple<string, TeamUpdateActivityRequest>> UpdateHandler(string memberID, TeamUpdateActivityContent content)
        {
            if (string.IsNullOrEmpty(content.ActID))
            {
                return Tuple.Create<string, TeamUpdateActivityRequest>(MessageHelper.Message.ResponseMessage.Team.ActivityIDEmpty, null);
            }

            TeamUpdateActivityRequest request = new TeamUpdateActivityRequest
            {
                MemberID = memberID,
                ActID = content.ActID,
                Action = (int)ActionType.Edit
            };

            IEnumerable<string> imgBase64s = (content.Routes != null && content.Routes.Any()) ? content.Routes.Select(route => route.Photo) : null;

            if (imgBase64s != null && imgBase64s.Any())
            {
                IEnumerable<string> imgUris = await this.uploadService.UploadTeamActivityImages(imgBase64s, true).ConfigureAwait(false);
                if (imgUris == null || !imgUris.Any())
                {
                    return Tuple.Create<string, TeamUpdateActivityRequest>(MessageHelper.Message.ResponseMessage.Upload.PhotoFail, null);
                }

                for (int i = 0; i < content.Routes.Count(); i++)
                {
                    //// 有的路線可能不附圖片，允許空字串
                    string imgUrl = imgUris.ElementAt(i);
                    Route route = content.Routes.ElementAt(i);
                    route.Photo = imgUrl;
                }
            }

            if (!string.IsNullOrEmpty(content.ActDate))
            {
                if (!DateTime.TryParse(content.ActDate, out DateTime actDate))
                {
                    return Tuple.Create<string, TeamUpdateActivityRequest>(MessageHelper.Message.ResponseMessage.Team.ActivityDateError, null);
                }

                request.ActDate = actDate.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrEmpty(content.MeetTime))
            {
                if (!DateTime.TryParse(content.MeetTime, out DateTime meetTime))
                {
                    return Tuple.Create<string, TeamUpdateActivityRequest>(MessageHelper.Message.ResponseMessage.Team.ActivityMeetTimeError, null);
                }

                request.MeetTime = meetTime.ToString("HH:mm:ss");
            }

            if (content.MaxAltitude > default(float))
            {
                request.MaxAltitude = content.MaxAltitude;
            }

            if (content.TotalDistance > default(float))
            {
                request.TotalDistance = content.TotalDistance;
            }

            if (!string.IsNullOrEmpty(content.Title))
            {
                request.Title = content.Title;
            }

            if (content.Routes != null && content.Routes.Any())
            {
                request.Route = JsonConvert.SerializeObject(content.Routes);
            }

            return Tuple.Create(string.Empty, request);
        }

        /// <summary>
        /// 新增車隊活動
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Add(string memberID, TeamAddActivityContent content)
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

                IEnumerable<string> imgBase64s = (content.Routes != null && content.Routes.Any()) ? content.Routes.Select(route => route.Photo) : null;

                if (imgBase64s != null && imgBase64s.Any())
                {
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

                    for (int i = 0; i < content.Routes.Count(); i++)
                    {
                        //// 有的路線可能不附圖片，允許空字串
                        string imgUrl = imgUris.ElementAt(i);
                        Route route = content.Routes.ElementAt(i);
                        route.Photo = imgUrl;
                    }
                }

                #endregion 上傳圖片

                #region 發送【更新活動】指令至後端

                TeamUpdateActivityRequest request = this.mapper.Map<TeamUpdateActivityRequest>(content);
                request.MemberID = memberID;
                request.MemberList = JsonConvert.SerializeObject(new string[] { memberID });
                request.Action = (int)ActionType.Add;

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
        /// 更新車隊活動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Edit(string memberID, TeamUpdateActivityContent content)
        {
            try
            {
                #region 處理更新資料

                Tuple<string, TeamUpdateActivityRequest> updateHandlerResult = await this.UpdateHandler(memberID, content).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(updateHandlerResult.Item1))
                {
                    this.logger.LogWarn("更新車隊活動資料結果", $"Result: 更新失敗({updateHandlerResult.Item1}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = updateHandlerResult.Item1
                    };
                }

                TeamUpdateActivityRequest request = updateHandlerResult.Item2;

                #endregion 處理更新資料

                #region 發送【更新活動】指令至後端

                CommandData<TeamUpdateActivityResponse> response = await this.serverService.DoAction<TeamUpdateActivityResponse>((int)TeamCommandIDType.UpdateActivity, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("更新車隊活動資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
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
                            ResultCode = (int)ResponseResultType.UpdateFail,
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
                this.logger.LogError("更新車隊資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 取得車隊活動明細資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetDetail(string memberID, TeamActivityDetailContent content)
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
                        ResultCode = (int)ResponseResultType.DenyAccess,
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
                teamActivityDetailView.ActionStatus = teamActivityDao.FounderID.Equals(memberID) ?
                                                      (int)ActivityActionStatusType.Delete : teamActivityDao.MemberList.Contains(memberID) ?
                                                      (int)ActivityActionStatusType.Cancel : (int)JoinStatusType.Join;
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
        /// 取得已參加的車隊活動列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetJoinList(string memberID)
        {
            try
            {
                IEnumerable<TeamActivityDao> teamActivityDaos = await this.teamActivityRepository.Get(memberID).ConfigureAwait(false);
                IEnumerable<TeamActivityListView> teamActivityListViews = teamActivityDaos.Select(dao =>
                {
                    TeamActivityListView teamActivityListView = this.mapper.Map<TeamActivityListView>(dao);
                    teamActivityListView.HasJoin = dao.MemberList.Contains(memberID) ? (int)JoinStatusType.Join : (int)JoinStatusType.None;
                    return teamActivityListView;
                });
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = teamActivityListViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得已參加的車隊活動列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得車隊的活動列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetList(string memberID, TeamContent content)
        {
            try
            {
                #region 驗證資料

                TeamContentValidator teamContentValidator = new TeamContentValidator();
                ValidationResult validationResult = teamContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("取得車隊的活動列表結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
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
                this.logger.LogError("取得車隊的活動列表發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 加入或離開車隊活動
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <param name="action">action</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> JoinOrLeave(string memberID, TeamActivityContent content, ActionType action)
        {
            try
            {
                #region 驗證資料

                TeamActivityContentValidator teamActivityContentValidator = new TeamActivityContentValidator();
                ValidationResult validationResult = teamActivityContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("加入或離開車隊活動結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} Action: {action}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 發送【加入或離開車隊活動】指令至後端

                TeamJoinOrLeaveActivityRequest request = this.mapper.Map<TeamJoinOrLeaveActivityRequest>(content);
                request.Action = (int)action;
                request.MemberID = memberID;
                CommandData<TeamJoinOrLeaveActivityResponse> response = await this.serverService.DoAction<TeamJoinOrLeaveActivityResponse>((int)TeamCommandIDType.JoinOrLeaveTeamActivity, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("加入或離開車隊活動結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} Action: {action}", null);
                switch (response.Data.Result)
                {
                    case (int)JoinOrLeaveTeamActivityResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)JoinOrLeaveTeamActivityResultType.Fail:
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

                #endregion 發送【加入或離開車隊活動】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("加入或離開車隊活動結果", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} Action: {action}", ex);
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