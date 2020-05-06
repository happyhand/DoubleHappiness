using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UploadFiles.Core.Extensions;
using UploadFiles.Service.Interfaces;

namespace UploadFiles.Api.Controllers.GoBike
{
    /// <summary>
    /// 騎乘圖像
    /// </summary>
    [Route("api/gobike/[controller]")]
    [ApiController]
    public class RideImageController : ControllerBase
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RideImageController");

        /// <summary>
        /// uploadFileService
        /// </summary>
        private readonly IUploadFileService uploadFileService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="uploadFileService">uploadFileService</param>
        public RideImageController(IUploadFileService uploadFileService)
        {
            this.uploadFileService = uploadFileService;
        }

        /// <summary>
        /// 騎乘圖像
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(IEnumerable<string> imgBase64s)
        {
            try
            {
                IEnumerable<string> imgUrls = this.uploadFileService.UploadImages("gobike", "ride", imgBase64s);
                if (!imgUrls.Any())
                {
                    return BadRequest("上傳騎乘圖像失敗.");
                }

                return Ok(imgUrls);
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳騎乘圖像發生錯誤", string.Empty, ex);
                return BadRequest("上傳騎乘圖像發生錯誤.");
            }
        }
    }
}