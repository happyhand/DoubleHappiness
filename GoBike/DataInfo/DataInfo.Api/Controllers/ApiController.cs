﻿using System;
using DataInfo.Core.Applibs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using DataInfo.Core.Extensions;
using NLog;

namespace DataInfo.Api.Controllers
{
    /// <summary>
    /// Base API
    /// </summary>
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ILogger logger = LogManager.GetLogger("ApiController");

        /// <summary>
        /// 取得 MemberID
        /// </summary>
        /// <returns>string</returns>
        protected string GetMemberID()
        {
            try
            {
                this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);
                if (string.IsNullOrEmpty(token))
                {
                    this.logger.LogWarn("取得 MemberID 發生錯誤", $"Result: 無效的 Jwt Token", null);
                    return string.Empty;
                }

                return JwtHelper.GetPayloadAppointValue<string>(token, "MemberID");
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得 MemberID 發生錯誤", string.Empty, ex);
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