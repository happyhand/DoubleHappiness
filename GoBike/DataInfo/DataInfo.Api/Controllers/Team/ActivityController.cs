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
    /// 車隊活動
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Team/[controller]")]
    public class ActivityController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamActivityController");

        /// <summary>
        /// teamActivityService
        /// </summary>
        private readonly ITeamActivityService teamActivityService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="teamActivityService">teamActivityService</param>
        public ActivityController(IJwtService jwtService, ITeamActivityService teamActivityService) : base(jwtService)
        {
            this.teamActivityService = teamActivityService;
        }

        /// <summary>
        /// 取得車隊活動列表
        /// </summary>
        /// <param name="teamID">teamID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{teamID}")]
        public async Task<IActionResult> Get(string teamID)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求取得車隊活動列表", $"MemberID: {memberID} TeamID: {teamID}", null);
                TeamActivityContent content = new TeamActivityContent() { TeamID = teamID };
                ResponseResult responseResult = await teamActivityService.GetTeamActivityList(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得車隊活動列表發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                });
            }
        }

        /// <summary>
        /// 取得車隊活動明細資料
        /// </summary>
        /// <param name="teamID">teamID</param>
        /// <param name="actID">actID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{teamID}/{actID}")]
        public async Task<IActionResult> Get(string teamID, string actID)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求取得車隊活動明細資料", $"MemberID: {memberID} TeamID: {teamID} ActID: {actID}", null);
                TeamActivityDetailContent content = new TeamActivityDetailContent() { TeamID = teamID, ActID = actID };
                ResponseResult responseResult = await teamActivityService.GetTeamActivityDetail(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得車隊活動明細資料發生錯誤", $"MemberID: {memberID} TeamID: {teamID} ActID: {actID}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                });
            }
        }

        /// <summary>
        /// 新增車隊活動
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(TeamAddActivityContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求新增車隊活動", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await teamActivityService.AddActivity(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求新增車隊活動發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                });
            }
        }
    }
}