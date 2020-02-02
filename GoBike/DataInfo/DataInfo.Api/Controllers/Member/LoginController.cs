using System;
using System.Threading.Tasks;
using DataInfo.Core.Applibs;
using DataInfo.Service.Interface.Member;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        private readonly ILogger<LoginController> logger;

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="memberService">memberService</param>
        public LoginController(ILogger<LoginController> logger, IMemberService memberService)
        {
            this.logger = logger;
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員登入 - 一般登入
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Route("api/Member/[controller]/[action]")]
        public async Task<IActionResult> Normal(MemberLoginPostData postData)
        {
            try
            {
                this.logger.LogInformation(this, "會員一般登入", $"Data:{JsonConvert.SerializeObject(postData)}");
                if (postData == null)
                {
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
                this.logger.LogError(this, "會員一般登入發生錯誤", $"Data:{JsonConvert.SerializeObject(postData)}", ex);
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
        }
    }
}