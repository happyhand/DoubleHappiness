using System;
using DataInfo.Api.Filters;
using DataInfo.Core.Extensions;
using DataInfo.Core.Applibs;
using DataInfo.Service.Interface.Member;
using Microsoft.AspNetCore.Mvc;
using NLog;

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
        [CheckLogin(true)]
        public void Get()
        {
            string memberID = this.HttpContext.Session.GetObject<string>(CommonFlagHelper.CommonFlag.SessionFlag.MemberID);
            try
            {
                this.logger.LogInfo("會員請求保持在線", $"MemberID: {memberID}", null);
                this.memberService.KeepOnline(this.HttpContext.Session, memberID);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求保持在線發生錯誤", $"MemberID: {memberID}", ex);
            }
        }
    }
}