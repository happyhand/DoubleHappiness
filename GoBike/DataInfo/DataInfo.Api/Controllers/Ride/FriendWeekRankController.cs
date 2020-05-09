using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Ride.Content;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Ride;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Ride
{
    /// <summary>
    /// 好友週里程排名
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Ride/[controller]")]
    public class FriendWeekRankController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RideFriendWeekRankController");

        /// <summary>
        /// rideService
        /// </summary>
        private readonly IRideService rideService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="rideService">rideService</param>
        public FriendWeekRankController(IJwtService jwtService, IRideService rideService) : base(jwtService)
        {
            this.rideService = rideService;
        }

        /// <summary>
        /// 好友週里程排名
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求好友週里程排名", $"MemberID: {memberID}", null);
                ResponseResult responseResult = await rideService.GetFriendWeekRank(memberID).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求好友週里程排名發生錯誤", $"MemberID: {memberID}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                });
            }
        }
    }
}