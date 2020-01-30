using DataInfo.Core.Applibs;
using DataInfo.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace DataInfo.Api.Filters
{
    /// <summary>
    /// 登入狀態檢測
    /// </summary>
    public class CheckLoginActionFilter : ActionFilterAttribute, IAsyncActionFilter
    {
        /// <summary>
        /// loginFlag
        /// </summary>
        private readonly bool loginFlag;

        /// <summary>
        /// 建構式
        /// </summary>
        public CheckLoginActionFilter()
        {
            this.loginFlag = false;
        }

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="loginFlag">loginFlag</param>
        public CheckLoginActionFilter(bool loginFlag)
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
            string memberID = context.HttpContext.Session.GetObject<string>(CommonFlagHelper.CommonFlag.SessionFlag.MemberID);
            BadRequestObjectResult badRequestObjectResult = null;
            if (this.loginFlag && string.IsNullOrEmpty(memberID))
            {
                badRequestObjectResult = new BadRequestObjectResult("會員尚未登入.");
            }
            else if (!this.loginFlag && !string.IsNullOrEmpty(memberID))
            {
                badRequestObjectResult = new BadRequestObjectResult("會員登入狀態發生錯誤.");
            }

            if (badRequestObjectResult != null)
            {
                await badRequestObjectResult.ExecuteResultAsync(context);
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}