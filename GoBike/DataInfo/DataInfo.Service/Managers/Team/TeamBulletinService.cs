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
using DataInfo.Service.Interfaces.Server;
using DataInfo.Service.Interfaces.Team;
using FluentValidation.Results;
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
    public class TeamBulletinService : ITeamBulletinService
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
        public TeamBulletinService(IMapper mapper, IServerService serverService, ITeamBulletinRepository teamBulletinRepository)
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
                return Tuple.Create<string, TeamUpdateBulletinRequest>(MessageHelper.Message.ResponseMessage.Team.BulletinIDEmpty, null);
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
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Add(string memberID, TeamAddBulletinContent content)
        {
            try
            {
                #region 驗證資料

                TeamAddBulletinContentValidator contentValidator = new TeamAddBulletinContentValidator();
                ValidationResult validationResult = contentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("新增車隊公告結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 發送【更新公告】指令至後端

                TeamUpdateBulletinRequest request = this.mapper.Map<TeamUpdateBulletinRequest>(content);
                request.MemberID = memberID;
                request.Action = (int)ActionType.Add;

                CommandData<TeamUpdateBulletinResponse> response = await this.serverService.DoAction<TeamUpdateBulletinResponse>((int)TeamCommandIDType.UpdateBulletin, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("新增車隊公告結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateBulletinResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Add.Success
                        };

                    case (int)UpdateBulletinResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.CreateFail,
                            Content = MessageHelper.Message.ResponseMessage.Add.Fail
                        };

                    case (int)UpdateBulletinResultType.AuthorityNotEnough:
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

                #endregion 發送【更新公告】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("新增車隊公告發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Add.Error
                };
            }
        }

        /// <summary>
        /// 刪除車隊公告
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Delete(string memberID, TeamBulletinContent content)
        {
            try
            {
                #region 驗證資料

                TeamBulletinContentValidator contentValidator = new TeamBulletinContentValidator();
                ValidationResult validationResult = contentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("刪除車隊公告結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 發送【更新公告】指令至後端

                TeamUpdateBulletinRequest request = this.mapper.Map<TeamUpdateBulletinRequest>(content);
                request.MemberID = memberID;
                request.Action = (int)ActionType.Delete;

                CommandData<TeamUpdateBulletinResponse> response = await this.serverService.DoAction<TeamUpdateBulletinResponse>((int)TeamCommandIDType.UpdateBulletin, CommandType.Team, request).ConfigureAwait(false);
                this.logger.LogInfo("刪除車隊公告結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateBulletinResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)UpdateBulletinResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    case (int)UpdateBulletinResultType.AuthorityNotEnough:
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
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };
                }

                #endregion 發送【更新公告】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除車隊公告發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 更新車隊公告
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Edit(string memberID, TeamUpdateBulletinContent content)
        {
            try
            {
                #region 處理更新資料

                Tuple<string, TeamUpdateBulletinRequest> updateHandlerResult = this.UpdateHandler(memberID, content);
                if (!string.IsNullOrEmpty(updateHandlerResult.Item1))
                {
                    this.logger.LogWarn("更新車隊公告資料結果", $"Result: 更新失敗({updateHandlerResult.Item1}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = updateHandlerResult.Item1
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
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)UpdateBulletinResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    case (int)UpdateBulletinResultType.AuthorityNotEnough:
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
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
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
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 取得車隊公告列表
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
                    this.logger.LogWarn("取得車隊公告列表結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 取得資料

                IEnumerable<TeamBulletinDao> teamBulletinDaos = await this.teamBulletinRepository.Get(memberID, content.TeamID).ConfigureAwait(false);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = this.mapper.Map<IEnumerable<TeamBulletinDao>>(teamBulletinDaos)
                };

                #endregion 取得資料
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊公告列表發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
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