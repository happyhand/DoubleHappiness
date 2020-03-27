using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員手機綁定
    /// </summary>
    [Route("api/Member/[controller]")]
    [Authorize]
    [ApiController]
    public class MobileBindController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MobileBindController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="memberService">memberService</param>
        public MobileBindController(IJwtService jwtService, IMemberService memberService) : base(jwtService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員手機綁定 - 發送驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberMobileBindContent content)
        {
            string email = this.GetEmail();
            try
            {
                if (content == null)
                {
                    this.logger.LogWarn("會員請求發送手機綁定驗證碼失敗", $"Content: 無資料 Email: {email}", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "未提供資料內容."
                    });
                }

                this.logger.LogInfo("會員請求發送手機綁定驗證碼", $"Email: {email} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResultDto responseResult = await this.memberService.SendMobileBindVerifierCode(email, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求發送手機綁定驗證碼發生錯誤", $"Email: {email} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "發送驗證碼發生錯誤."
                });
            }
        }
    }
}