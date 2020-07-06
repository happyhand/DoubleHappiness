using DataInfo.Core.Models.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataInfo.Core.Extensions;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using DataInfo.Service.Interfaces.Common;

namespace DataInfo.Api.Filters
{
    /// <summary>
    /// API 內容檢測
    /// </summary>
    public class ContentAttribute : IAsyncActionFilter
    {
        /// <summary>
        /// jwtService
        /// </summary>
        private readonly IJwtService jwtService;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("ContentAttribute");

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        public ContentAttribute(IJwtService jwtService)
        {
            this.jwtService = jwtService;
        }

        /// <summary>
        /// 檢測篩檢
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="next">next</param>
        /// <returns>Task</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                string[] errorDatas = context.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage.Split("|");
                string message = errorDatas.ElementAtOrDefault(0);
                string data = errorDatas.ElementAtOrDefault(1);
                ResponseResult responseResult = new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status400BadRequest,
                    ResultMessage = message
                };

                context.Result = new BadRequestObjectResult(responseResult);
                string memberID = this.jwtService.GetPayloadAppointValue(context.HttpContext.User, "MemberID");
                this.logger.LogWarn("API 請求內容資料驗證失敗", $"MemberID: {memberID} Paht: {context.HttpContext.Request.Path} Message: {message} Data: {data}", null);
                return;
            }

            await next();
        }
    }
}