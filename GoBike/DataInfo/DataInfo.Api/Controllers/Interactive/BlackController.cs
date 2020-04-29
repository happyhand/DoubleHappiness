﻿using DataInfo.Core.Models.Enum;
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
    /// 會員黑名單
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BlackController : JwtController
    {
        /// <summary>
        /// InteractiveService
        /// </summary>
        private readonly IInteractiveService InteractiveService;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("BlackController");

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="InteractiveService">InteractiveService</param>
        public BlackController(IJwtService jwtService, IInteractiveService InteractiveService) : base(jwtService)
        {
            this.InteractiveService = InteractiveService;
        }

        /// <summary>
        /// 會員黑名單 - 移除黑名單
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(InteractiveContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求移除黑名單", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.InteractiveService.DeleteInteractive(memberID, content).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求移除黑名單發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "更新資料發生錯誤."
                });
            }
        }

        /// <summary>
        /// 會員黑名單 - 取得黑名單列表
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            try
            {
                ResponseResult responseResult = await this.InteractiveService.GetBlackList(memberID).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得黑名單列表發生錯誤", $"MemberID: {memberID}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                });
            }
        }

        /// <summary>
        /// 會員黑名單 - 更新互動資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(InteractiveContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求更新互動資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.InteractiveService.UpdateInteractive(memberID, content, (int)InteractiveType.Black).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求更新互動資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return Ok(new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "更新資料發生錯誤."
                });
            }
        }
    }
}