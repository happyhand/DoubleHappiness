using System;
using System.Threading.Tasks;
using DataInfo.Core.Applibs;
using DataInfo.Service.Interface.Member;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員搜尋
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
        /// 會員搜尋
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(MemberSearchPostData postData)
        {
            try
            {
                //this.logger.LogInformation(this, "會員搜尋", $"Data:{JsonConvert.SerializeObject(postData)}");
                if (postData == null)
                {
                    return BadRequest("無會員搜尋資料.");
                }

                ResponseResultDto responseResult = string.IsNullOrEmpty(postData.Email) ?
                    await memberService.Search(postData.MemberID).ConfigureAwait(false) :
                    await memberService.Search(postData.Email).ConfigureAwait(false);
                if (responseResult.Ok)
                {
                    return Ok(responseResult.Data);
                }

                return BadRequest(responseResult.Data);
            }
            catch (Exception ex)
            {
                //this.logger.LogError(this, "會員搜尋發生錯誤", $"Data:{JsonConvert.SerializeObject(postData)}", ex);
                return BadRequest("會員搜尋發生錯誤.");
            }
        }

        /// <summary>
        /// 會員搜尋 Post 資料
        /// </summary>
        public class MemberSearchPostData
        {
            /// <summary>
            /// Gets or sets Email
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Gets or sets MemberID
            /// </summary>
            public long MemberID { get; set; }
        }
    }
}