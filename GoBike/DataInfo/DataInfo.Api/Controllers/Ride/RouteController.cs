using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Ride.Content;
using DataInfo.Core.Models.Enum;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Ride;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Ride
{
    /// <summary>
    /// 騎乘路線功能
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Ride/[controller]")]
    public class RouteController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RouteController");

        /// <summary>
        /// rideService
        /// </summary>
        private readonly IRideService rideService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="rideService">rideService</param>
        public RouteController(IJwtService jwtService, IRideService rideService) : base(jwtService)
        {
            this.rideService = rideService;
        }

        /// <summary>
        /// 騎乘路線功能 - 新增騎乘路線資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(AddRideRouteDataContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求新增騎乘路線資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await rideService.AddRideRouteData(content, memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求新增騎乘路線資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 騎乘路線功能 - 取得騎乘路線資料
        /// </summary>
        /// <param name="rideID">rideID</param>
        /// <param name="index">index</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{rideID}/{index}")]
        public async Task<IActionResult> Get(string rideID, int index)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求取得騎乘路線資料", $"MemberID: {memberID} RideID: {rideID} index: {index}", null);
                ResponseResult responseResult = await this.rideService.GetRideRoute(memberID, rideID, index).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得騎乘路線資料發生錯誤", $"MemberID: {memberID} RideID: {rideID} index: {index}", ex);
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