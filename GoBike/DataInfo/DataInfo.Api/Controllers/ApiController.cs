﻿using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NLog;
using System;

namespace DataInfo.Api.Controllers
{
    /// <summary>
    /// Base API
    /// </summary>
    [ApiController]
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// jwtService
        /// </summary>
        private readonly IJwtService jwtService;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("ApiController");

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        public ApiController(IJwtService jwtService)
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
        /// 取得 MemberID
        /// </summary>
        /// <returns>string</returns>
        protected string GetMemberID()
        {
            try
            {
                return this.jwtService.GetPayloadAppointValue(User, "MemberID");
            }
            catch (Exception ex)
            {
                this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);
                this.logger.LogError("取得 MemberID 發生錯誤", $"Token: {token}", ex);
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