using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Enum;
using DataInfo.Service.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System.Threading.Tasks;

namespace DataInfo.Api.Filters
{
    /// <summary>
    /// 手機綁定內容檢測
    /// </summary>
    public class MobileBindAttribute : ActionFilterAttribute, IAsyncActionFilter
    {
        /// <summary>
        /// jwtService
        /// </summary>
        private readonly IJwtService jwtService;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MobileBindAttribute");

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        public MobileBindAttribute(IJwtService jwtService)
        {
            this.jwtService = jwtService;
        }

        /// <summary>
        /// 檢測篩檢
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="next">next</param>
        /// <returns>Task</returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string mobile = this.jwtService.GetPayloadAppointValue(context.HttpContext.User, "Mobile");
            if (!string.IsNullOrEmpty(mobile))
            {
                string memberID = this.jwtService.GetPayloadAppointValue(context.HttpContext.User, "MemberID");
                ResponseResult responseResult = new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status409Conflict,
                    ResultMessage = ResponseErrorMessageType.MobileRepeat.ToString()
                };

                context.Result = new ObjectResult(responseResult)
                {
                    StatusCode = StatusCodes.Status409Conflict,
                };
                this.logger.LogWarn("手機綁定請求內容資料驗證失敗", $"MemberID: {memberID} Mobile: {mobile}", null);
                return;
            }

            await next();
        }
    }
}