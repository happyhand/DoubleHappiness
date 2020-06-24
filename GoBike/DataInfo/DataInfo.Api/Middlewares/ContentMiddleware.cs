using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataInfo.Core.Extensions;
using NLog;
using Newtonsoft.Json;
using DataInfo.Core.Models.Dto.Member.Content;
using System.IO;
using NLog.Internal;
using System.Text;

namespace DataInfo.Api.Middlewares
{
    /// <summary>
    /// API 內容中間層
    /// </summary>
    public class ContentMiddleware
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("ContentMiddleware");

        /// <summary>
        /// RequestDelegate
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="next">next</param>
        public ContentMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// API 調用
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            //try
            //{
            //    context.Request.EnableBuffering();
            //    var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            //    await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            //    var bodyAsText = Encoding.UTF8.GetString(buffer);
            //    context.Request.Body.Seek(0, SeekOrigin.Begin);
            //    MemberLoginContent content = JsonConvert.DeserializeObject<MemberLoginContent>(bodyAsText);

            // MemberLoginContentValidator memberLoginContentValidator = new
            // MemberLoginContentValidator(); ValidationResult validationResult =
            // memberLoginContentValidator.Validate(content); if (!validationResult.IsValid) {
            // string errorMessgae = validationResult.Errors[0].ErrorMessage;
            // this.logger.LogWarn("會員登入結果(一般登入)", $"Result: 驗證失敗({errorMessgae}) Email:
            // {content.Email} Password: {content.Password}", null); return new ResponseResult() {
            // Result = false, ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage =
            // errorMessgae }; }

            //    this.logger.LogInfo("API 調用", $"bodyAsText: {bodyAsText} {content.Email}", null);
            //    await this.next(context);
            //}
            //catch (Exception ex)
            //{
            //    this.logger.LogError("API 調用 Error", string.Empty, ex);
            //}
        }
    }
}