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
    /// 會員註冊
    /// </summary>
    [Route("api/Member/[controller]")]
    [ApiController]
    public class RegisterController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger<RegisterController> logger;

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="memberService">memberService</param>
        public RegisterController(ILogger<RegisterController> logger, IMemberService memberService)
        {
            this.logger = logger;
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberRegisterPostData postData)
        {
            try
            {
                this.logger.LogInformation(this, "會員註冊", $"Data:{JsonConvert.SerializeObject(postData)}");
                if (postData == null)
                {
                    return BadRequest("無會員註冊資料.");
                }

                ResponseResultDto responseResult = await memberService.Register(postData.Email, postData.Password, true, string.Empty, string.Empty).ConfigureAwait(false);
                if (responseResult.Ok)
                {
                    return Ok(responseResult.Data);
                }

                return BadRequest(responseResult.Data);
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "會員註冊發生錯誤", $"Data:{JsonConvert.SerializeObject(postData)}", ex);
                return BadRequest("會員註冊發生錯誤.");
            }
        }

        /// <summary>
        /// 會員註冊 Post 資料
        /// </summary>
        public class MemberRegisterPostData
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