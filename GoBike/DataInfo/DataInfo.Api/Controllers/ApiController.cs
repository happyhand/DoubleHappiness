using Microsoft.AspNetCore.Mvc;

namespace DataInfo.Api.Controllers
{
    /// <summary>
    /// Base API
    /// </summary>
    [ApiController]
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// 執行 BadRequest 回覆加密
        /// </summary>
        /// <param name="error">error</param>
        /// <returns>BadRequestObjectResult</returns>
        public override BadRequestObjectResult BadRequest(object error)
        {
            //if (error is string)
            //{
            //    return base.BadRequest(Utility.EncryptAES(error as string));
            //}

            return base.BadRequest(error);
        }

        /// <summary>
        /// 執行 OK 回覆加密
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>OkObjectResult</returns>
        public override OkObjectResult Ok(object value)
        {
            //if (value is string)
            //{
            //    return base.Ok(Utility.EncryptAES(value as string));
            //}

            return base.Ok(value);
        }
    }
}