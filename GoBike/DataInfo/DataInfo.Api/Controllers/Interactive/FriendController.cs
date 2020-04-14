using DataInfo.Core.Models.Enum;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Interactive
{
    /// <summary>
    /// 會員好友
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FriendController : ApiController
    {
        /// <summary>
        /// InteractiveService
        /// </summary>
        private readonly IInteractiveService InteractiveService;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("FriendController");

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="InteractiveService">InteractiveService</param>
        public FriendController(IJwtService jwtService, IInteractiveService InteractiveService) : base(jwtService)
        {
            this.InteractiveService = InteractiveService;
        }

        /// <summary>
        /// 會員好友 - 取得好友列表
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            try
            {
                ResponseResultDto responseResult = await this.InteractiveService.GetFriendList(memberID).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得好友列表發生錯誤", $"MemberID: {memberID}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                });
            }
        }

        /// <summary>
        /// 會員好友 - 更新互動資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(InteractiveContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                if (content == null)
                {
                    this.logger.LogWarn("會員請求更新互動資料失敗", $"Content: 無資料 MemberID: {memberID}", null);
                    return Ok(new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "未提供資料內容."
                    });
                }

                this.logger.LogInfo("會員請求更新互動資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResultDto responseResult = await this.InteractiveService.UpdateInteractive(memberID, content, (int)InteractiveType.Friend).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求更新互動資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "更新互動資料發生錯誤."
                });
            }
        }
    }
}