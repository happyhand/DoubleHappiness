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
    /// 車隊資訊
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Team/[controller]")]
    public class InfoController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamInfoController");

        /// <summary>
        /// teamService
        /// </summary>
        private readonly ITeamService teamService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="teamService">teamService</param>
        public InfoController(IJwtService jwtService, ITeamService teamService) : base(jwtService)
        {
            this.teamService = teamService;
        }

        /// <summary>
        /// 取得車隊資訊
        /// </summary>
        /// <param name="teamID">teamID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{TeamID}")]
        public async Task<IActionResult> Get(string teamID)
        {
            //try
            //{
            //    this.logger.LogInfo("會員請求取得車隊資訊", $"TeamID: {teamID}", null);
            //    ResponseResult responseResult = await teamService.Create(memberID, content).ConfigureAwait(false);
            //    return Ok(responseResult);
            //}
            //catch (Exception ex)
            //{
            //    this.logger.LogError("會員請求取得車隊資訊發生錯誤", $"TeamID: {teamID}", ex);
            //    return Ok(new ResponseResult()
            //    {
            //        Result = false,
            //        ResultCode = (int)ResponseResultType.UnknownError,
            //        Content = MessageHelper.Message.ResponseMessage.Get.Error
            //    });
            //}

            return null;
        }
    }
}