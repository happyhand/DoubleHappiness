﻿using Microsoft.AspNetCore.Mvc;
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
    /// 會員圖像
    /// </summary>
    [Route("api/gobike/[controller]")]
    [ApiController]
    public class MemberImageController : ControllerBase
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberImageController");

        /// <summary>
        /// uploadFileService
        /// </summary>
        private readonly IUploadFileService uploadFileService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="uploadFileService">uploadFileService</param>
        public MemberImageController(IUploadFileService uploadFileService)
        {
            this.uploadFileService = uploadFileService;
        }

        ///// <summary>
        ///// 上傳檔案 - 上傳會員圖像
        ///// </summary>
        ///// <returns>IActionResult</returns>
        //[HttpPost]
        //[DisableFormValueModelBindingFilter]
        //public async Task<IActionResult> MemberImages()
        //{
        //    try
        //    {
        //        IEnumerable<string> filePaths = await this.uploadFileService.UploadImages(this.Request, "gobike", "member");
        //        if (!filePaths.Any())
        //        {
        //            return BadRequest("上傳會員圖像失敗.");
        //        }

        //        return Ok(filePaths);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("上傳會員圖像發生錯誤", string.Empty, ex);
        //        return BadRequest("上傳會員圖像發生錯誤.");
        //    }
        //}

        ///// <summary>
        ///// 會員圖像
        ///// </summary>
        ///// <returns>IActionResult</returns>
        //[HttpPost]
        //public async Task<IActionResult> Post(IEnumerable<string> imgBase64s)
        //{
        //    try
        //    {
        //        IEnumerable<string> imgUrls = this.uploadFileService.UploadImages("gobike", "member", imgBase64s);
        //        if (!imgUrls.Any())
        //        {
        //            return BadRequest("上傳會員圖像失敗.");
        //        }

        //        return Ok(imgUrls);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("上傳會員圖像發生錯誤", string.Empty, ex);
        //        return BadRequest("上傳會員圖像發生錯誤.");
        //    }
        //}
    }
}