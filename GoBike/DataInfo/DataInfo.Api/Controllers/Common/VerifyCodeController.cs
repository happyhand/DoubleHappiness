using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Common;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Enum;
using DataInfo.Service.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Common
{
    /// <summary>
    /// 驗證碼
    /// </summary>
    [Route("api/Common/[controller]")]
    [ApiController]
    public class VerifyCodeController : BaseController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("VerifyCodeController");

        /// <summary>
        /// verifyCodeService
        /// </summary>
        private readonly IVerifyCodeService verifyCodeService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="verifyCodeService">verifyCodeService</param>
        public VerifyCodeController(IVerifyCodeService verifyCodeService)
        {
            this.verifyCodeService = verifyCodeService;
        }

        /// <summary>
        /// 驗證驗證碼是否存在
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(VerifyCodeContent content)
        {
            try
            {
                this.logger.LogInfo("會員請求驗證驗證碼是否存在", $"Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.verifyCodeService.Validate(content.Email, content.VerifierCode, false).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求驗證驗證碼是否存在發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                ResponseResult errorResponseResult = new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };

                return this.ResponseHandler(errorResponseResult);
            }
        }
    }
}