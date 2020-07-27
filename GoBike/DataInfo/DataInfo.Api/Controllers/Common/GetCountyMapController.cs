using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;

namespace DataInfo.Api.Controllers.Common
{
    /// <summary>
    /// 取得市區資料列表
    /// </summary>
    [Route("api/Common/[controller]")]
    [ApiController]
    public class GetCountyMapController : BaseController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("GetCountyMapController");

        /// <summary>
        /// 取得市區資料列表
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = AppSettingHelper.Appsetting.CountyMap
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得市區資料列表發生錯誤", string.Empty, ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }
    }
}