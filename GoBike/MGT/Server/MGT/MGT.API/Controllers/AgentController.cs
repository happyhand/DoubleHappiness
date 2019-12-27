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
    /// 代理商功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AgentController : ControllerBase
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger<AgentController> logger;

        /// <summary>
        /// mgtService
        /// </summary>
        private readonly IMgtService mgtService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="mgtService">mgtService</param>
        public AgentController(ILogger<AgentController> logger, IMgtService mgtService)
        {
            this.logger = logger;
            this.mgtService = mgtService;
        }

        /// <summary>
        /// 代理商 - 新增代理商資料
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public IActionResult Add(AgentPostData postData)
        {
            try
            {
                this.mgtService.AddAgent(postData.Account, postData.Password);
                return Ok("新增代理商資料成功.");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Add Agent Error >>> Account:{postData.Account} Password:{postData.Password}\n{ex}");
                return BadRequest("新增代理商資料發生錯誤.");
            }
        }

        /// <summary>
        /// 代理商 - 取得代理商資料
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Get(GetAgentPostData postData)
        {
            try
            {
                Tuple<AgentData, string> result = await this.mgtService.GetAgent(postData.Id);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(result.Item1);
                }

                return BadRequest(result.Item2);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Agent Error >>> ID:{postData.Id}\n{ex}");
                return BadRequest("取得代理商資料發生錯誤.");
            }
        }

        /// <summary>
        /// 代理商 - 代理商登入
        /// </summary>
        /// <param name="postData">postData</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Login(AgentPostData postData)
        {
            try
            {
                string result = await this.mgtService.AgentLogin(postData.Account, postData.Password);
                if (string.IsNullOrEmpty(result))
                {
                    return Ok("代理商登入成功.");
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Agent Login Error >>> Account:{postData.Account} Password:{postData.Password}\n{ex}");
                return BadRequest("代理商登入發生錯誤.");
            }
        }

        /// <summary>
        /// 新增代理商資料 Post Data
        /// </summary>
        public class AgentPostData
        {
            /// <summary>
            /// Gets or sets Account
            /// </summary>
            public string Account { get; set; }

            /// <summary>
            /// Gets or sets Password
            /// </summary>
            public string Password { get; set; }
        }

        /// <summary>
        /// 取得代理商資料 Post Data
        /// </summary>
        public class GetAgentPostData
        {
            /// <summary>
            /// Gets or sets Id
            /// </summary>
            public long Id { get; set; }
        }
    }
}