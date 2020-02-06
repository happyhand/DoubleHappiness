using DataInfo.Core.Extensions;
using DataInfo.Core.Resource;
using DataInfo.Service.Interface.Member;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 保持在線
    /// </summary>
    [Route("api/Member/[controller]")]
    [ApiController]
    public class KeepOnlineController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("KeepOnlineController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="memberService">memberService</param>
        public KeepOnlineController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 保持在線
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string memberID = this.HttpContext.Session.GetObject<string>(CommonFlagHelper.CommonFlag.SessionFlag.MemberID);
                ResponseResultDto recordSessionIDResult = await memberService.ExtendSessionIDExpire(memberID, HttpContext.Session.Id).ConfigureAwait(false);
                if (!recordSessionIDResult.Ok)
                {
                    return BadRequest("Keep Online Fail.");
                }
                return Ok("Keep Online");
            }
            catch (Exception ex)
            {
                //this.logger.LogError(this, "會員保持在線發生錯誤", string.Empty, ex);
                return BadRequest("Keep Online Error.");
            }
        }
    }
}