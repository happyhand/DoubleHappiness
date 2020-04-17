using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Response;
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
    public class ForgetPasswordController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("ForgetPasswordController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="memberService">memberService</param>
        public ForgetPasswordController(IJwtService jwtService, IMemberService memberService) : base(jwtService)
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
                if (content == null)
                {
                    this.logger.LogWarn("會員請求重置密碼失敗", "Content: 無資料", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "未提供資料內容."
                    });
                }

                this.logger.LogInfo("會員請求重置密碼", $"Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResultDto responseResult = await this.memberService.ResetPassword(content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求重置密碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "更新資料發生錯誤."
                });
            }
        }

        /// <summary>
        /// 會員忘記密碼 - 發送驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberForgetPasswordContent content)
        {
            try
            {
                if (content == null)
                {
                    this.logger.LogWarn("會員請求發送忘記密碼驗證碼失敗", "Content: 無資料", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "未提供資料內容."
                    });
                }

                this.logger.LogInfo("會員請求發送忘記密碼驗證碼", $"Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResultDto responseResult = await this.memberService.SendForgetPasswordVerifierCode(content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求發送忘記密碼驗證碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
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