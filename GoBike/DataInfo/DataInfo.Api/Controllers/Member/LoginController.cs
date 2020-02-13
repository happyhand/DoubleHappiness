using System;
using System.Threading.Tasks;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interface.Member;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員登入功能
    /// </summary>
    [ApiController]
    public class LoginController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("LoginController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="memberService">memberService</param>
        public LoginController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員登入 - FB登入
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Route("api/Member/[controller]/[action]")]
        public async Task<IActionResult> FB(MemberLoginPostData postData)
        {
            try
            {
                this.logger.LogInfo("會員請求登入(FB登入)", $"Data: {JsonConvert.SerializeObject(postData)}", null);
                if (postData == null)
                {
                    this.logger.LogWarn("會員請求登入失敗(FB登入)", "Data: 無資料", null);
                    return BadRequest("無會員登入資料.");
                }

                ResponseResultDto responseResult = await memberService.LoginWithFB(this.HttpContext.Session, postData.Email, postData.Token).ConfigureAwait(false);
                if (responseResult.Ok)
                {
                    return Ok(responseResult.Data);
                }

                return BadRequest(responseResult.Data);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求登入發生錯誤(FB登入)", $"Data: {JsonConvert.SerializeObject(postData)}", ex);
                return BadRequest("會員登入發生錯誤.");
            }
        }

        /// <summary>
        /// 會員登入 - Google登入
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Route("api/Member/[controller]/[action]")]
        public async Task<IActionResult> Google(MemberLoginPostData postData)
        {
            try
            {
                this.logger.LogInfo("會員請求登入(Google登入)", $"Data: {JsonConvert.SerializeObject(postData)}", null);
                if (postData == null)
                {
                    this.logger.LogWarn("會員請求登入失敗(Google登入)", "Data: 無資料", null);
                    return BadRequest("無會員登入資料.");
                }

                ResponseResultDto responseResult = await memberService.LoginWithGoogle(this.HttpContext.Session, postData.Email, postData.Token).ConfigureAwait(false);
                if (responseResult.Ok)
                {
                    return Ok(responseResult.Data);
                }

                return BadRequest(responseResult.Data);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求登入發生錯誤(Google登入)", $"Data: {JsonConvert.SerializeObject(postData)}", ex);
                return BadRequest("會員登入發生錯誤.");
            }
        }

        /// <summary>
        /// 會員登入 - 一般登入
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Route("api/Member/[controller]")]
        public async Task<IActionResult> Normal(MemberLoginPostData postData)
        {
            try
            {
                this.logger.LogInfo("會員請求登入(一般登入)", $"Data: {JsonConvert.SerializeObject(postData)}", null);
                if (postData == null)
                {
                    this.logger.LogWarn("會員請求登入失敗(一般登入)", "Data: 無資料", null);
                    return BadRequest("無會員登入資料.");
                }

                ResponseResultDto responseResult = await memberService.Login(this.HttpContext.Session, postData.Email, postData.Password).ConfigureAwait(false);
                if (responseResult.Ok)
                {
                    return Ok(responseResult.Data);
                }

                return BadRequest(responseResult.Data);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求登入發生錯誤(一般登入)", $"Data: {JsonConvert.SerializeObject(postData)}", ex);
                return BadRequest("會員登入發生錯誤.");
            }
        }

        /// <summary>
        /// 會員登入 Post 資料
        /// </summary>
        public class MemberLoginPostData
        {
            /// <summary>
            /// Gets or sets Email
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Gets or sets Password
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Gets or sets Token
            /// </summary>
            public string Token { get; set; }
        }
    }
}