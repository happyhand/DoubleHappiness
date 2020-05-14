using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Ride;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Ride
{
    /// <summary>
    /// 騎乘記錄
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Ride/[controller]")]
    public class RecordController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RideRecordController");

        /// <summary>
        /// rideService
        /// </summary>
        private readonly IRideService rideService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="rideService">rideService</param>
        public RecordController(IJwtService jwtService, IRideService rideService) : base(jwtService)
        {
            this.rideService = rideService;
        }

        /// <summary>
        /// 取得騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{memberID}")]
        private async Task<IActionResult> GetData(string memberID)
        {
            try
            {
                this.logger.LogInfo("會員請求取得騎乘記錄", $"MemberID: {memberID}", null);
                ResponseResult responseResult = await rideService.GetRideRecord(memberID).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得騎乘記錄發生錯誤", $"MemberID: {memberID}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                });
            }
        }

        /// <summary>
        /// 取得騎乘記錄
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            return this.GetData(memberID);
        }

        /// <summary>
        /// 取得騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{memberID}")]
        public Task<IActionResult> Get(string memberID)
        {
            return this.GetData(memberID);
        }
    }
}