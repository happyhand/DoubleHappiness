using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 保持在線
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Member/[controller]")]
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
            string memberID = this.GetMemberID();
            try
            {
                ResponseResultDto responseResult = await this.memberService.KeepOnline(memberID).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求保持在線發生錯誤", $"MemberID: {memberID}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "保持在線發生錯誤."
                });
            }
        }
    }
}