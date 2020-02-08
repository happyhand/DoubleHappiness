using System;
using DataInfo.Api.Filters;
using DataInfo.Core.Extensions;
using DataInfo.Core.Resource;
using DataInfo.Service.Interface.Member;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員登出
    /// </summary>
    [Route("api/Member/[controller]")]
    [ApiController]
    public class LogoutController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("LogoutController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="memberService">memberService</param>
        public LogoutController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員登出
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [CheckLogin(true)]
        public void Get()
        {
            string memberID = this.HttpContext.Session.GetObject<string>(CommonFlagHelper.CommonFlag.SessionFlag.MemberID);
            try
            {
                this.logger.LogInfo("會員請求登出", $"MemberID: {memberID}", null);
                this.memberService.Logout(this.HttpContext.Session, memberID);
            }
            catch (Exception ex)
            {
                this.logger.LogInfo("會員請求登出發生錯誤", $"MemberID: {memberID}", ex);
            }
        }
    }
}