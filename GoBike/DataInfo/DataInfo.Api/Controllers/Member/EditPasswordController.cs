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

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員修改密碼
    /// </summary>
    [Route("api/Member/[controller]")]
    [Authorize]
    [ApiController]
    public class EditPasswordController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("EditPasswordController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="memberService">memberService</param>
        public EditPasswordController(IJwtService jwtService, IMemberService memberService) : base(jwtService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員修改密碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPatch]
        public async Task<IActionResult> Patch(MemberEditPasswordContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                if (content == null)
                {
                    this.logger.LogWarn("會員請求修改密碼失敗", $"Content: 無資料 MemberID: {memberID}", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "未提供資料內容."
                    });
                }

                this.logger.LogInfo("會員請求修改密碼", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResultDto responseResult = await this.memberService.EditPassword(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求修改密碼發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "更新資料發生錯誤."
                });
            }
        }
    }
}