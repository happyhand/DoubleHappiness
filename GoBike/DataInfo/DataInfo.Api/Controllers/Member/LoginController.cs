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
    /// 會員登入
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
        /// <param name="jwtService">jwtService</param>
        /// <param name="memberService">memberService</param>
        public LoginController(IJwtService jwtService, IMemberService memberService) : base(jwtService)
        {
            this.memberService = memberService;
        }

        ///// <summary>
        ///// 會員登入 - FB登入
        ///// </summary>
        ///// <param name="postData">postData</param>
        ///// <returns>IActionResult</returns>
        //[HttpPost]
        //[Route("api/Member/[controller]/[action]")]
        //public async Task<IActionResult> FB(MemberLoginPostData postData)
        //{
        //    try
        //    {
        //        this.logger.LogInfo("會員請求登入(FB登入)", $"Data: {JsonConvert.SerializeObject(postData)}", null);
        //        if (postData == null)
        //        {
        //            this.logger.LogWarn("會員請求登入失敗(FB登入)", "Data: 無資料", null);
        //            return BadRequest("無會員登入資料.");
        //        }

        // ResponseResultDto responseResult = await memberService.LoginWithFB(postData.Email,
        // postData.Token).ConfigureAwait(false); if (responseResult.Ok) { return
        // Ok(responseResult.Data); }

        //        return BadRequest(responseResult.Data);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("會員請求登入發生錯誤(FB登入)", $"Data: {JsonConvert.SerializeObject(postData)}", ex);
        //        return BadRequest("會員登入發生錯誤.");
        //    }
        //}

        ///// <summary>
        ///// 會員登入 - Google登入
        ///// </summary>
        ///// <param name="postData">postData</param>
        ///// <returns>IActionResult</returns>
        //[HttpPost]
        //[Route("api/Member/[controller]/[action]")]
        //public async Task<IActionResult> Google(MemberLoginPostData postData)
        //{
        //    try
        //    {
        //        this.logger.LogInfo("會員請求登入(Google登入)", $"Data: {JsonConvert.SerializeObject(postData)}", null);
        //        if (postData == null)
        //        {
        //            this.logger.LogWarn("會員請求登入失敗(Google登入)", "Data: 無資料", null);
        //            return BadRequest("無會員登入資料.");
        //        }

        // ResponseResultDto responseResult = await memberService.LoginWithGoogle(postData.Email,
        // postData.Token).ConfigureAwait(false); if (responseResult.Ok) { return
        // Ok(responseResult.Data); }

        //        return BadRequest(responseResult.Data);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("會員請求登入發生錯誤(Google登入)", $"Data: {JsonConvert.SerializeObject(postData)}", ex);
        //        return BadRequest("會員登入發生錯誤.");
        //    }
        //}

        /// <summary>
        /// 會員登入 - 重新登入
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [Authorize]
        [Route("api/Member/[controller]")]
        public async Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求登入(重新登入)", $"MemberID: {memberID}", null);
                ResponseResultDto responseResult = await memberService.Relogin(memberID).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求登入發生錯誤(重新登入)", $"MemberID: {memberID}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "登入發生錯誤."
                });
            }
        }

        /// <summary>
        /// 會員登入 - 一般登入
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Route("api/Member/[controller]")]
        public async Task<IActionResult> Post(MemberLoginContent content)
        {
            try
            {
                if (content == null)
                {
                    this.logger.LogWarn("會員請求登入失敗(一般登入)", "Content: 無資料", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "無登入資料."
                    });
                }

                this.logger.LogInfo("會員請求登入(一般登入)", $"Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResultDto responseResult = await memberService.Login(content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求登入發生錯誤(一般登入)", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "登入發生錯誤."
                });
            }
        }
    }
}