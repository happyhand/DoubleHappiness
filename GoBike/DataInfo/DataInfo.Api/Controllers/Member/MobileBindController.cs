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

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員手機綁定
    /// </summary>
    [Route("api/Member/[controller]")]
    [Authorize]
    [ApiController]
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
            string mobile = this.GetMobile();
            try
            {
                if (!string.IsNullOrEmpty(mobile))
                {
                    this.logger.LogWarn("會員請求手機綁定失敗", $"Result: 已綁定手機 MemberID: {memberID} Mobile: {mobile} Content: {JsonConvert.SerializeObject(content)}", null);
                    return Ok(new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Member.MemberHasBindMobile
                    });
                }

                this.logger.LogInfo("會員請求手機綁定", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.memberService.MobileBind(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求手機綁定發生錯誤", $"MemberID: {memberID} Mobile: {mobile} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
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
            string mobile = this.GetMobile();
            try
            {
                if (!string.IsNullOrEmpty(mobile))
                {
                    this.logger.LogWarn("會員請求發送手機綁定驗證碼失敗", $"Result: 會員已綁定手機 MemberID: {memberID} Mobile: {mobile} Content: {JsonConvert.SerializeObject(content)}", null);
                    return Ok(new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Member.MemberHasBindMobile
                    });
                }

                this.logger.LogInfo("會員請求發送手機綁定驗證碼", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.memberService.SendMobileBindVerifierCode(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求發送手機綁定驗證碼發生錯誤", $"MemberID: {memberID} Mobile: {mobile} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.VerifyCode.SendVerifyCodeError
                });
            }
        }
    }
}