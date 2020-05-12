using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.User.Content;
using DataInfo.Service.Interfaces.User;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.User
{
    /// <summary>
    /// 使用者登入
    /// </summary>
    [ApiController]
    [Route("api/user/[controller]")]
    public class UserLoginController : ApiController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("UserLoginController");

        /// <summary>
        /// userService
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="userService">userService</param>
        public UserLoginController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(UserLoginContent content)
        {
            try
            {
                this.logger.LogInfo("使用者請求登入", $"Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await userService.Login(content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("使用者請求登入發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Login.Error
                });
            }
        }
    }
}