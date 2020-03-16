using MGT.Repository.Models.Data;
using MGT.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGT.API.Controllers
{
    /// <summary>
    /// 會員功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MemberController : ControllerBase
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger<MemberController> logger;

        /// <summary>
        /// mgtService
        /// </summary>
        private readonly IMgtService mgtService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="mgtService">mgtService</param>
        public MemberController(ILogger<MemberController> logger, IMgtService mgtService)
        {
            this.logger = logger;
            this.mgtService = mgtService;
        }

        /// <summary>
        /// 會員 - 新增會員資料
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public IActionResult Add()
        {
            try
            {
                this.mgtService.AddMember();
                return Ok("新增會員資料成功.");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Add Member Error >>>\n{ex}");
                return BadRequest("新增會員資料發生錯誤.");
            }
        }

        /// <summary>
        /// 會員 - 取得會員資料
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Get(GetMemberByIDPostData postData)
        {
            try
            {
                Tuple<MemberData, string> result = await this.mgtService.GetMemberByMemberID(postData.MemberID);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(result.Item1);
                }

                return BadRequest(result.Item2);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Member By Member ID Error >>> MemberID:{postData.MemberID}\n{ex}");
                return BadRequest("取得會員資料發生錯誤.");
            }
        }

        /// <summary>
        /// 取得會員資料 Post Data
        /// </summary>
        public class GetMemberByIDPostData
        {
            /// <summary>
            /// Gets or sets Id
            /// </summary>
            public int MemberID { get; set; }
        }
    }
}