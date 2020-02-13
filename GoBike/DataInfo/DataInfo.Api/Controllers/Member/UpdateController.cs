using System;
using System.Threading.Tasks;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interface.Member;
using DataInfo.Service.Models.Member.data;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員更新功能
    /// </summary>
    [Route("api/Member/[controller]/[action]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("UpdateController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="memberService">memberService</param>
        public UpdateController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 會員更新 - 基本資料
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Info(MemberUpdateInfoPostData postData)
        {
            try
            {
                this.logger.LogInfo("會員更新資料", $"Data: {JsonConvert.SerializeObject(postData)}", null);
                if (postData == null)
                {
                    this.logger.LogWarn("會員更新資料失敗", "Data: 無資料", null);
                    return BadRequest("無會員更新資料.");
                }

                string memberID = this.HttpContext.Session.GetObject<string>(CommonFlagHelper.CommonFlag.SessionFlag.MemberID);
                MemberDto memberDto = new MemberDto()
                {
                    MemberID = memberID,
                    Birthday = postData.Birthday,
                    BodyHeight = postData.BodyHeight,
                    BodyWeight = postData.BodyWeight,
                    Gender = postData.Gender,
                    FrontCoverUrl = postData.FrontCoverUrl,
                    PhotoUrl = postData.PhotoUrl
                };

                ResponseResultDto responseResult = await memberService.UpdateInfo(memberDto).ConfigureAwait(false);
                if (responseResult.Ok)
                {
                    return Ok(responseResult.Data);
                }

                return BadRequest(responseResult.Data);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員更新資料發生錯誤", $"Data: {JsonConvert.SerializeObject(postData)}", ex);
                return BadRequest("會員更新資料發生錯誤.");
            }
        }

        /// <summary>
        /// 會員更新基本資料 Post 資料
        /// </summary>
        public class MemberUpdateInfoPostData
        {
            /// <summary>
            /// Gets or sets Birthday
            /// </summary>
            public DateTime Birthday { get; set; }

            /// <summary>
            /// Gets or sets BodyHeight
            /// </summary>
            public double BodyHeight { get; set; }

            /// <summary>
            /// Gets or sets BodyWeight
            /// </summary>
            public double BodyWeight { get; set; }

            /// <summary>
            /// Gets or sets FrontCoverUrl
            /// </summary>
            public string FrontCoverUrl { get; set; }

            /// <summary>
            /// Gets or sets Gender
            /// </summary>
            public int Gender { get; set; }

            /// <summary>
            /// Gets or sets Nickname
            /// </summary>
            public string Nickname { get; set; }

            /// <summary>
            /// Gets or sets PhotoUrl
            /// </summary>
            public string PhotoUrl { get; set; }
        }
    }
}