using DataInfo.Core.Extensions;
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
    /// 會員編輯資訊
    /// </summary>
    [Route("api/Member/[controller]")]
    [Authorize]
    [ApiController]
    public class EditInfoController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("EditInfoController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="memberService">memberService</param>
        public EditInfoController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員編輯資訊
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberEditInfoContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                if (content == null)
                {
                    this.logger.LogWarn("請求會員編輯資訊失敗", $"Content: 無資料 MemberID: {memberID}", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "未提供編輯內容."
                    });
                }

                this.logger.LogInfo("請求會員編輯資訊", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResultDto responseResult = await this.memberService.EditInfo(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員編輯資訊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "編輯資訊發生錯誤."
                });
            }
        }
    }
}