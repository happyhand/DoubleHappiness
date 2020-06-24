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

namespace DataInfo.Api.Filters
{
    /// <summary>
    /// API 內容檢測
    /// </summary>
    public class ContentAttribute : IAsyncActionFilter
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("ContentAttribute");

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
                string[] errorMessages = context.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage.Split("|");
                string errorMessage = errorMessages.ElementAtOrDefault(0);
                string content = errorMessages.ElementAtOrDefault(1);
                ResponseResult responseResult = new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status400BadRequest,
                    ResultMessage = errorMessage
                };

                context.Result = new BadRequestObjectResult(responseResult);
                this.logger.LogWarn("API 內容驗證失敗", $"Paht: {context.HttpContext.Request.Path} ErrorMessage: {errorMessage} Content: {content}", null);
                return;
            }

            await next();
        }
    }
}