using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Enum;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Team
{
    /// <summary>
    /// 車隊公告
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Team/[controller]")]
    public class BulletinController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamBulletinController");

        /// <summary>
        /// teamBulletinService
        /// </summary>
        private readonly ITeamBulletinService teamBulletinService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="teamBulletinService">teamBulletinService</param>
        public BulletinController(IJwtService jwtService, ITeamBulletinService teamBulletinService) : base(jwtService)
        {
            this.teamBulletinService = teamBulletinService;
        }

        /// <summary>
        /// 刪除車隊公告
        /// </summary>
        /// <param name="bulletinID">bulletinID</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{bulletinID}")]
        public async Task<IActionResult> Delete(string bulletinID)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求刪除車隊公告", $"MemberID: {memberID} BulletinID: {bulletinID}", null);
                ResponseResult responseResult = await this.teamBulletinService.Delete(memberID, bulletinID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求刪除車隊公告發生錯誤", $"MemberID: {memberID} BulletinID: {bulletinID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 取得車隊公告列表
        /// </summary>
        /// <param name="teamID">teamID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{teamID}")]
        public async Task<IActionResult> Get(string teamID)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求取得車隊公告列表", $"MemberID: {memberID} TeamID: {teamID}", null);
                ResponseResult responseResult = await this.teamBulletinService.GetList(memberID, teamID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得車隊公告列表發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 更新車隊公告
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPatch]
        public async Task<IActionResult> Patch(TeamUpdateBulletinContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求更新車隊公告", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.teamBulletinService.Edit(content, memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求更新車隊公告發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 新增車隊公告
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(TeamAddBulletinContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求新增車隊公告", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.teamBulletinService.Add(content, memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求新增車隊公告發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }
    }
}