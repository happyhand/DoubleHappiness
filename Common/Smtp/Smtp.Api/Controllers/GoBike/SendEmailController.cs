using Microsoft.AspNetCore.Mvc;
using NLog;
using Smtp.Core.Extensions;
using Smtp.Service.Interfaces;
using Smtp.Service.Models;
using System;
using System.Threading.Tasks;

namespace Smtp.Api.Controllers.GoBike
{
    /// <summary>
    /// 發送郵件
    /// </summary>
    [Route("api/gobike/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("SendEmailController");

        /// <summary>
        /// smtpService
        /// </summary>
        private readonly ISmtpService smtpService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="smtpService">smtpService</param>
        public SendEmailController(ISmtpService smtpService)
        {
            this.smtpService = smtpService;
        }

        /// <summary>
        /// 發送郵件
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(EmailContext emailContext)
        {
            try
            {
                if (emailContext == null)
                {
                    this.logger.LogWarn("請求發送郵件失敗", $"Content: 無資料", null);
                    return BadRequest("未提供郵件內容.");
                }

                string result = await this.smtpService.SendEmail(emailContext);
                if (!string.IsNullOrEmpty(result))
                {
                    return BadRequest(result);
                }

                return Ok("發送郵件成功.");
            }
            catch (Exception ex)
            {
                this.logger.LogError("請求發送郵件發生錯誤", string.Empty, ex);
                return BadRequest("發送郵件發生錯誤.");
            }
        }
    }
}