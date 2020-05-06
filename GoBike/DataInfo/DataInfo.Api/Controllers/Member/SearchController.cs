using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Enum;
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
    /// 搜尋會員
    /// </summary>
    [Route("api/Member/[controller]")]
    [Authorize]
    [ApiController]
    public class SearchController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberSearchController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="memberService">memberService</param>
        public SearchController(IJwtService jwtService, IMemberService memberService) : base(jwtService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 搜尋會員 - 取得本身資料
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            try
            {
                MemberSearchContent content = new MemberSearchContent() { SearchKey = memberID };
                ResponseResult responseResult = await this.memberService.StrictSearch(content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得本身資料發生錯誤", $"MemberID: {memberID}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                });
            }
        }

        /// <summary>
        /// 搜尋會員 - 搜尋其他會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="useFuzzySearch">useFuzzySearch</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{searchKey}/{useFuzzySearch}")]
        public async Task<IActionResult> Get(string searchKey, int useFuzzySearch)
        {
            string memberID = this.GetMemberID();
            try
            {
                ResponseResult responseResult;
                MemberSearchContent content = new MemberSearchContent() { SearchKey = searchKey, UseFuzzySearch = useFuzzySearch };
                if (useFuzzySearch == (int)SearchType.Fuzzy)
                {
                    responseResult = await this.memberService.FuzzySearch(content, memberID).ConfigureAwait(false);
                }
                else
                {
                    responseResult = await this.memberService.StrictSearch(content, memberID).ConfigureAwait(false);
                }

                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求搜尋其他會員資料發生錯誤", $"MemberID: {memberID} SearchKey: {searchKey} UseFuzzySearch: {useFuzzySearch}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                });
            }
        }
    }
}