using System;
using System.Threading.Tasks;
using DataInfo.Core.Extensions;
using DataInfo.Service.Enums;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 搜尋會員
    /// </summary>
    [Route("api/Member/[controller]")]
    [Authorize]
    [ApiController]
    public class SearchController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("SearchController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="memberService">memberService</param>
        public SearchController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 搜尋會員 - 取得會員本身資料
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            try
            {
                ResponseResultDto responseResult = await this.memberService.StrictSearch(memberID).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("請求取得會員本身資料發生錯誤", $"MemberID: {memberID}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                });
            }
        }

        /// <summary>
        /// 搜尋會員 - 搜尋會員資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberSearchContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                if (content == null)
                {
                    this.logger.LogWarn("請求搜尋會員資料失敗", $"Content: 無資料 MemberID: {memberID}", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "未提供搜尋內容."
                    });
                }

                ResponseResultDto responseResult;
                if (content.UseFuzzySearch == (int)SearchType.Fuzzy)
                {
                    responseResult = await this.memberService.FuzzySearch(content.SearchKey, memberID).ConfigureAwait(false);
                }
                else
                {
                    responseResult = await this.memberService.StrictSearch(content.SearchKey, memberID).ConfigureAwait(false);
                }

                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋會員資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                });
            }
        }
    }
}