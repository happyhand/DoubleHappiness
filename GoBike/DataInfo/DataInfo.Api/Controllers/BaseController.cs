using DataInfo.Core.Models.Dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace DataInfo.Api.Controllers
{
    /// <summary>
    /// Base Controller
    /// </summary>
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// API 回覆處理
        /// </summary>
        /// <param name="result">result</param>
        /// <returns>IActionResult</returns>
        protected IActionResult ResponseHandler(ResponseResult result)
        {
            //return this.StatusCode(result.ResultCode, Utility.EncryptAES(JsonConvert.SerializeObject(result)));
            return this.StatusCode(result.ResultCode, result);
        }
    }
}