using System;
using System.Threading.Tasks;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員註冊
    /// </summary>
    [ApiController]
    [Route("api/Member/[controller]")]
    public class RegisterController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RegisterController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="memberService">memberService</param>
        public RegisterController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberRegisterContent content)
        {
            try
            {
                if (content == null)
                {
                    this.logger.LogWarn("會員請求註冊失敗", "Content: 無資料", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "未提供資料內容."
                    });
                }

                this.logger.LogInfo("會員請求註冊", $"Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResultDto responseResult = await memberService.Register(content.Email, content.Password, content.ConfirmPassword, true, string.Empty, string.Empty).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求註冊發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "註冊發生錯誤."
                });
            }
        }
    }
}