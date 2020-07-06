using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Enum;
using DataInfo.Service.Interfaces.Member;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員忘記密碼
    /// </summary>
    [ApiController]
    [Route("api/Member/[controller]")]
    public class ForgetPasswordController : BaseController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberForgetPasswordController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="memberService">memberService</param>
        public ForgetPasswordController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員忘記密碼 - 重置密碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPatch]
        public async Task<IActionResult> Patch(MemberForgetPasswordContent content)
        {
            try
            {
                this.logger.LogInfo("會員請求重置密碼", $"Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.memberService.ResetPassword(content).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求重置密碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 會員忘記密碼 - 請求發送驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberRequestForgetPasswordContent content)
        {
            try
            {
                this.logger.LogInfo("會員請求發送忘記密碼驗證碼", $"Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.memberService.SendForgetPasswordVerifierCode(content).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求發送忘記密碼驗證碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }
    }
}