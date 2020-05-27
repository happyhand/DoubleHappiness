﻿using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Api.Controllers.Team
{
    /// <summary>
    /// 車隊下拉選單
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Team/[controller]")]
    public class DropMenuController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamDropMenuController");

        /// <summary>
        /// teamService
        /// </summary>
        private readonly ITeamService teamService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="teamService">teamService</param>
        public DropMenuController(IJwtService jwtService, ITeamService teamService) : base(jwtService)
        {
            this.teamService = teamService;
        }

        /// <summary>
        /// 取得車隊下拉選單
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求取得車隊下拉選單", $"MemberID: {memberID}", null);
                ResponseResult responseResult = await teamService.GetTeamDropMenu(memberID).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得車隊下拉選單發生錯誤", $"MemberID: {memberID}", ex);
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