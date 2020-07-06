using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Enum;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Member
{
    /// <summary>
    /// 會員名片資訊
    /// </summary>
    [Route("api/Member/[controller]")]
    [Authorize]
    [ApiController]
    public class CardInfoController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberCardInfoController");

        /// <summary>
        /// memberService
        /// </summary>
        private readonly IMemberService memberService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="memberService">memberService</param>
        public CardInfoController(IJwtService jwtService, IMemberService memberService) : base(jwtService)
        {
            this.memberService = memberService;
        }

        /// <summary>
        /// 取得會員名片資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>IActionResult</returns>
        private async Task<IActionResult> GetData(string memberID)
        {
            try
            {
                MemberCardInfoContent content = new MemberCardInfoContent() { MemberID = memberID };
                ResponseResult responseResult = await this.memberService.GetCardInfo(content).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得名片資訊發生錯誤", $"MemberID: {memberID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 取得會員名片資訊
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            return this.GetData(memberID);
        }

        /// <summary>
        /// 取得會員名片資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{memberID}")]
        public Task<IActionResult> Get(string memberID)
        {
            //// TODO 待確認有沒有要限制其他會員可不可以搜尋到資料
            return this.GetData(memberID);
        }
    }
}