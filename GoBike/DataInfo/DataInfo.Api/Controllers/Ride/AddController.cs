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
using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Dto.Ride.Content;

namespace DataInfo.Api.Controllers.Ride
{
    /// <summary>
    /// 新增騎乘資料
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Ride/[controller]")]
    public class AddController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RideAddController");

        /// <summary>
        /// rideService
        /// </summary>
        private readonly IRideService rideService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="rideService">rideService</param>
        public AddController(IJwtService jwtService, IRideService rideService) : base(jwtService)
        {
            this.rideService = rideService;
        }

        ///// <summary>
        ///// 會員騎乘 - 取得騎乘資料列表
        ///// </summary>
        ///// <param name="memberID"></param>
        ///// <returns></returns>
        //private async Task<IActionResult> GetData(string memberID)
        //{
        //    try
        //    {
        //        ResponseResult responseResult = await this.rideService.GetRideDataListOfMember(memberID).ConfigureAwait(false);
        //        return Ok(responseResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("會員請求取得騎乘資料列表發生錯誤", $"MemberID: {memberID}", ex);
        //        return Ok(new ResponseResult()
        //        {
        //            Result = false,
        //            ResultCode = (int)ResponseResultType.UnknownError,
        //            Content = MessageHelper.Message.ResponseMessage.Get.Error
        //        });
        //    }
        //}

        ///// <summary>
        ///// 會員騎乘 - 取得騎乘資料列表
        ///// </summary>
        ///// <returns>IActionResult</returns>
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    string memberID = this.GetMemberID();
        //    return await this.GetData(memberID).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// 會員騎乘 - 取得騎乘資料列表
        ///// </summary>
        ///// <returns>IActionResult</returns>
        //[HttpGet("{memberID:string}")]
        //public async Task<IActionResult> Get(string memberID)
        //{
        //    return await this.GetData(memberID).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// 會員騎乘 - 更新騎乘資料 (TODO)
        ///// </summary>
        ///// <param name="content">content</param>
        ///// <returns>IActionResult</returns>
        //[HttpPatch]
        //public async Task<IActionResult> Patch(RideUpdateInfoContent content)
        //{
        //    string memberID = this.GetMemberID();
        //    try
        //    {
        //        if (content == null)
        //        {
        //            this.logger.LogWarn("會員請求更新騎乘資料失敗", $"Content: 無資料 MemberID: {memberID}", null);
        //            return Ok(new ResponseResultDto()
        //            {
        //                Result = false,
        //                ResultCode = (int)ResponseResultType.InputError,
        //                Content = "未提供資料內容."
        //            });
        //        }

        //        this.logger.LogInfo("會員請求更新騎乘資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
        //        ResponseResultDto responseResult = await rideService.UpdateRideData(memberID, content).ConfigureAwait(false);
        //        return Ok(responseResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("會員更新騎乘資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
        //        return Ok(new ResponseResultDto()
        //        {
        //            Result = false,
        //            ResultCode = (int)ResponseResultType.UnknownError,
        //            Content = "更新資料發生錯誤."
        //        });
        //    }
        //}

        /// <summary>
        /// 新增騎乘資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(AddRideInfoContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求新增騎乘資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await rideService.AddRideData(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求新增騎乘資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Add.Error
                });
            }
        }
    }
}