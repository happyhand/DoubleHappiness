using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
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
    public class GetCountryMapController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("GetCountryMapController");

        /// <summary>
        /// 取得市區資料列表
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(AppSettingHelper.Appsetting.CountryMap);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得市區資料列表發生錯誤", string.Empty, ex);
                return BadRequest("取得資料發生錯誤.");
            }
        }
    }
}