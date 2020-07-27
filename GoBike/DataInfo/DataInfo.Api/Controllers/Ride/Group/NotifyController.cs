using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Enum;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Ride;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Ride
{
    /// <summary>
    /// 發送組隊騎乘通知
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Ride/Group/[controller]")]
    public class NotifyController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RideGroupNotifyController");

        /// <summary>
        /// rideService
        /// </summary>
        private readonly IRideService rideService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="rideService">rideService</param>
        public NotifyController(IJwtService jwtService, IRideService rideService) : base(jwtService)
        {
            this.rideService = rideService;
        }

        /// <summary>
        /// 發送組隊騎乘通知
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求發送組隊騎乘通知", $"MemberID: {memberID}", null);
                ResponseResult responseResult = await rideService.SendNotify(memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求發送組隊騎乘通知發生錯誤", $"MemberID: {memberID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }
    }
}