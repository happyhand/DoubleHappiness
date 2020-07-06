using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;
using DataInfo.Core.Applibs;
using Microsoft.AspNetCore.Http;
using DataInfo.Core.Models.Enum;
using DataInfo.Api.Filters;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員手機綁定
    /// </summary>
    [Route("api/Member/[controller]")]
    [Authorize]
    [ApiController]
    [TypeFilter(typeof(MobileBindAttribute))]
    public class MobileBindController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberMobileBindController");

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
        /// 會員手機綁定 - 手機綁定
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPatch]
        public async Task<IActionResult> Patch(MemberMobileBindContent content)
        {
            string memberID = this.GetMemberID();
            string email = this.GetEmail();
            try
            {
                this.logger.LogInfo("會員請求手機綁定", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.memberService.MobileBind(memberID, content).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求手機綁定發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 會員手機綁定 - 請求發送驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberRequestMobileBindContent content)
        {
            string memberID = this.GetMemberID();
            string email = this.GetEmail();
            try
            {
                this.logger.LogInfo("會員請求發送手機綁定驗證碼", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.memberService.SendMobileBindVerifierCode(memberID, email, content).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求發送手機綁定驗證碼發生錯誤", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)}", ex);
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