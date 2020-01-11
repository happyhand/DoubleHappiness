using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGT.API.Controllers
{
    /// <summary>
    /// 測試 API
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// GET
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return Ok("Get API.");
        }
    }
}