using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UploadFiles.Core.Extensions;
using UploadFiles.Core.Models.Dto.Image.Content;
using UploadFiles.Service.Interfaces;

namespace UploadFiles.Api.Controllers
{
    /// <summary>
    /// 圖像上傳
    /// </summary>
    [Route("api/uploadFile/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("ImageController");

        /// <summary>
        /// uploadFileService
        /// </summary>
        private readonly IUploadFileService uploadFileService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="uploadFileService">uploadFileService</param>
        public ImageController(IUploadFileService uploadFileService)
        {
            this.uploadFileService = uploadFileService;
        }

        /// <summary>
        /// 圖像上傳
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(ImageContent content)
        {
            try
            {
                IEnumerable<string> imgUrls = this.uploadFileService.UploadImages(content);
                if (!imgUrls.Any())
                {
                    return BadRequest("圖像上傳失敗.");
                }

                return Ok(imgUrls);
            }
            catch (Exception ex)
            {
                this.logger.LogError("圖像上傳發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return BadRequest("圖像上傳發生錯誤.");
            }
        }
    }
}