using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Team
{
    /// <summary>
    /// 車隊功能
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Team")]
    public class TeamController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamController");

        /// <summary>
        /// teamService
        /// </summary>
        private readonly ITeamService teamService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="teamService">teamService</param>
        public TeamController(IJwtService jwtService, ITeamService teamService) : base(jwtService)
        {
            this.teamService = teamService;
        }

        /// <summary>
        /// 車隊功能 - 解散車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(TeamContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求解散車隊", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await teamService.Disband(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求解散車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Add.Error
                });
            }
        }

        /// <summary>
        /// 車隊功能 - 取得車隊資訊
        /// </summary>
        /// <param name="teamID">teamID</param>
        /// <param name="infoType">infoType</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{teamID}/{infoType}")]
        public async Task<IActionResult> Get(string teamID, int infoType)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求取得車隊資訊", $"MemberID: {memberID} TeamID: {teamID}", null);
                TeamContent content = new TeamContent() { TeamID = teamID };
                if (infoType == 1)
                {
                    return Ok(await teamService.GetTeamSetting(memberID, teamID).ConfigureAwait(false));
                }
                else
                {
                    return Ok(await teamService.GetTeamInfo(memberID, content).ConfigureAwait(false));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得車隊資訊發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                });
            }
        }

        /// <summary>
        /// 車隊功能 - 更新車隊資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPatch]
        public async Task<IActionResult> Patch(TeamEditContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求更新車隊資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await teamService.Edit(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求更新車隊資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                });
            }
        }

        /// <summary>
        /// 車隊功能 - 建立車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(TeamCreateContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求建立車隊", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await teamService.Create(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求建立車隊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Add.Error
                });
            }
        }
    }
}