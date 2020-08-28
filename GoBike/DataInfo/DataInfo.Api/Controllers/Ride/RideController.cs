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
    /// 騎乘功能
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Ride")]
    public class RideController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RideController");

        /// <summary>
        /// rideService
        /// </summary>
        private readonly IRideService rideService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="rideService">rideService</param>
        public RideController(IJwtService jwtService, IRideService rideService) : base(jwtService)
        {
            this.rideService = rideService;
        }

        /// <summary>
        /// 騎乘功能 - 取得騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>IActionResult</returns>
        private async Task<IActionResult> GetData(string memberID)
        {
            try
            {
                this.logger.LogInfo("會員請求取得騎乘記錄", $"MemberID: {memberID}", null);
                ResponseResult responseResult = await this.rideService.GetRideRecord(memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得騎乘記錄發生錯誤", $"MemberID: {memberID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 騎乘功能 - 取得騎乘明細記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="rideID">rideID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{memberID}/{rideID}")]
        public async Task<IActionResult> Get(string memberID, string rideID)
        {
            try
            {
                this.logger.LogInfo("會員請求取得騎乘明細記錄", $"MemberID: {memberID} RideID: {rideID}", null);
                ResponseResult responseResult = await this.rideService.GetRideDetailRecord(memberID, rideID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得騎乘明細記錄發生錯誤", $"MemberID: {memberID} RideID: {rideID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 騎乘功能 - 取得騎乘記錄
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            return this.GetData(memberID);
        }

        /// <summary>
        /// 騎乘功能 - 取得騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{memberID}")]
        public Task<IActionResult> Get(string memberID)
        {
            return this.GetData(memberID);
        }

        /// <summary>
        /// 騎乘功能 - 新增騎乘資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(AddRideDataContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求新增騎乘資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await rideService.AddRideData(content, memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求新增騎乘資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
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