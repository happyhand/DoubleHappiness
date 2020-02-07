using System;
using System.Threading.Tasks;
using DataInfo.Api.Filters;
using DataInfo.Core.Extensions;
using DataInfo.Core.Resource;
using DataInfo.Service.Interface.Member;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 搜尋會員
    /// </summary>
    [Route("api/Member/[controller]")]
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
        /// <param name="logger">logger</param>
        /// <param name="memberService">memberService</param>
        public SearchController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 搜尋會員
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [CheckLogin(true)]
        public async Task<IActionResult> Post(MemberSearchPostData postData)
        {
            try
            {
                this.logger.LogInfo("請求搜尋會員", $"Data: {JsonConvert.SerializeObject(postData)}", null);
                if (postData == null || string.IsNullOrEmpty(postData.SearchKey))
                {
                    this.logger.LogWarn("搜尋會員失敗", "Data: 無資料", null);
                    return BadRequest("無搜尋會員資料.");
                }

                string memberID = this.HttpContext.Session.GetObject<string>(CommonFlagHelper.CommonFlag.SessionFlag.MemberID);
                ResponseResultDto responseResult = await memberService.Search(postData.SearchKey, memberID).ConfigureAwait(false);
                if (responseResult.Ok)
                {
                    return Ok(responseResult.Data);
                }

                return BadRequest(responseResult.Data);
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋會員發生錯誤", $"Data: {JsonConvert.SerializeObject(postData)}", ex);
                return BadRequest("搜尋會員發生錯誤.");
            }
        }

        /// <summary>
        /// 會員搜尋 Post 資料
        /// </summary>
        public class MemberSearchPostData
        {
            /// <summary>
            /// Gets or sets SearchKey
            /// </summary>
            public string SearchKey { get; set; }
        }
    }
}