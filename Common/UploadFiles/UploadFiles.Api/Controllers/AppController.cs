using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UploadFiles.Api.Controllers
{
    /// <summary>
    /// APP 測試 Api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Upload File Service");
        }
    }
}