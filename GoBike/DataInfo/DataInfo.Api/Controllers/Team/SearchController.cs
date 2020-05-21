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
    /// 搜尋車隊
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Team/[controller]")]
    public class SearchController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamSearchController");

        /// <summary>
        /// teamService
        /// </summary>
        private readonly ITeamService teamService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="teamService">teamService</param>
        public SearchController(IJwtService jwtService, ITeamService teamService) : base(jwtService)
        {
            this.teamService = teamService;
        }

        /// <summary>
        /// 搜尋車隊
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{searchKey}")]
        public async Task<IActionResult> Get(string searchKey)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求搜尋車隊", $"MemberID: {memberID} SearchKey: {searchKey}", null);
                ResponseResult responseResult = await teamService.SearchTeam(memberID, searchKey).ConfigureAwait(false);
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求搜尋車隊發生錯誤", $"MemberID: {memberID} SearchKey: {searchKey}", ex);
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