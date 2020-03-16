using System.Threading.Tasks;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DataInfo.Api.Filters
{
    /// <summary>
    /// 登入狀態檢測
    /// </summary>
    public class CheckLoginAttribute : ActionFilterAttribute, IAsyncActionFilter
    {
        /// <summary>
        /// loginFlag
        /// </summary>
        private readonly bool loginFlag;

        /// <summary>
        /// 建構式
        /// </summary>
        public CheckLoginAttribute()
        {
            this.loginFlag = false;
        }

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="loginFlag">loginFlag</param>
        public CheckLoginAttribute(bool loginFlag)
        {
            this.loginFlag = loginFlag;
        }

        /// <summary>
        /// 檢測篩檢
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="next">next</param>
        /// <returns>Task</returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            BadRequestObjectResult badRequestObjectResult = null;
            if (context != null)
            {
                string memberID = "123456";
                if (this.loginFlag && string.IsNullOrEmpty(memberID))
                {
                    badRequestObjectResult = new BadRequestObjectResult("會員尚未登入.");
                }
                else if (!this.loginFlag && !string.IsNullOrEmpty(memberID))
                {
                    badRequestObjectResult = new BadRequestObjectResult("會員登入狀態發生錯誤.");
                }
            }
            else
            {
                badRequestObjectResult = new BadRequestObjectResult("ActionExecutingContext 發生錯誤.");
            }

            if (badRequestObjectResult != null)
            {
                await badRequestObjectResult.ExecuteResultAsync(context).ConfigureAwait(false);
                return;
            }

            await base.OnActionExecutionAsync(context, next).ConfigureAwait(false);
        }
    }
}