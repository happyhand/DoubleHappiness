using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NLog;
using System;

namespace DataInfo.Api.Controllers
{
    /// <summary>
    /// Jwt Controller
    /// </summary>
    [ApiController]
    public class JwtController : ApiController
    {
        /// <summary>
        /// jwtService
        /// </summary>
        private readonly IJwtService jwtService;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("JwtController");

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        public JwtController(IJwtService jwtService)
        {
            this.jwtService = jwtService;
        }

        /// <summary>
        /// 取得 Email
        /// </summary>
        /// <returns>string</returns>
        protected string GetEmail()
        {
            try
            {
                return this.jwtService.GetPayloadAppointValue(User, "Email");
            }
            catch (Exception ex)
            {
                this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);
                this.logger.LogError("取得 Email 發生錯誤", $"Token: {token}", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 取得 Mobile
        /// </summary>
        /// <returns>string</returns>
        protected string GetMobile()
        {
            try
            {
                return this.jwtService.GetPayloadAppointValue(User, "Mobile");
            }
            catch (Exception ex)
            {
                this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);
                this.logger.LogError("取得 Mobile 發生錯誤", $"Token: {token}", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 取得 UserID
        /// </summary>
        /// <returns>string</returns>
        protected string GetUserID()
        {
            try
            {
                return this.jwtService.GetPayloadAppointValue(User, "UserID");
            }
            catch (Exception ex)
            {
                this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);
                this.logger.LogError("取得 UserID 發生錯誤", $"Token: {token}", ex);
                return string.Empty;
            }
        }
    }
}