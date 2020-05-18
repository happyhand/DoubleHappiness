using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;

namespace DataInfo.Api.Controllers.Common
{
    /// <summary>
    /// 取得版本號
    /// </summary>
    [Route("api/Common/[controller]")]
    [ApiController]
    public class VersionController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("VersionController");

        /// <summary>
        /// 取得版本號
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "1.0.0.3" //// 大版本、Hotfix、Bug、develop
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得版本號發生錯誤", string.Empty, ex);
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