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
    /// 加入車隊活動
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Team/[controller]")]
    public class JoinActivityController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamJoinActivityController");

        /// <summary>
        /// teamActivityService
        /// </summary>
        private readonly ITeamActivityService teamActivityService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="teamActivityService">teamActivityService</param>
        public JoinActivityController(IJwtService jwtService, ITeamActivityService teamActivityService) : base(jwtService)
        {
            this.teamActivityService = teamActivityService;
        }

        /// <summary>
        /// 取消加入車隊活動
        /// </summary>
        /// <param name="teamID">teamID</param>
        /// <param name="actID">actID</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{teamID}/{actID}")]
        public async Task<IActionResult> Delete(string teamID, string actID)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求取消加入車隊活動", $"MemberID: {memberID} TeamID: {teamID} ActID: {actID}", null);
                if (string.IsNullOrEmpty(teamID) || string.IsNullOrEmpty(actID))
                {
                    return this.ResponseHandler(new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = string.IsNullOrEmpty(teamID) ? ResponseErrorMessageType.TeamIDEmpty.ToString() : ResponseErrorMessageType.ActIDEmpty.ToString()
                    });
                }

                ResponseResult responseResult = await teamActivityService.JoinOrLeave(new TeamActivityContent() { TeamID = teamID, ActID = actID }, memberID, ActionType.Delete).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取消加入車隊活動發生錯誤", $"MemberID: {memberID} TeamID: {teamID} ActID: {actID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 加入車隊活動
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(TeamActivityContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求加入車隊活動", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await teamActivityService.JoinOrLeave(content, memberID, ActionType.Add).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求加入車隊活動發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
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