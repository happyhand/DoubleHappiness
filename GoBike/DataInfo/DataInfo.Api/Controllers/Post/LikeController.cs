using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Post.Content;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Enum;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Post;
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
    /// 貼文點讚功能
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Post/[controller]")]
    public class LikeController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("PostLikeController");

        /// <summary>
        /// postService
        /// </summary>
        private readonly IPostService postService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="postService">postService</param>
        public LikeController(IJwtService jwtService, IPostService postService) : base(jwtService)
        {
            this.postService = postService;
        }

        /// <summary>
        /// 貼文點讚功能 - 刪除點讚數
        /// </summary>
        /// <param name="postID">postID</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{postID}")]
        public async Task<IActionResult> Delete(string postID)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求刪除貼文資料", $"MemberID: {memberID} PostID: {postID}", null);
                if (string.IsNullOrEmpty(postID))
                {
                    return this.ResponseHandler(new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = ResponseErrorMessageType.PostIDEmpty.ToString()
                    });
                }

                ResponseResult responseResult = await this.postService.DeletePostLikeData(memberID, postID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求刪除貼文資料發生錯誤", $"MemberID: {memberID} PostID: {postID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 貼文點讚功能 - 新增點讚數
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(AddPostLikeContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求新增貼文點讚數資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.postService.AddPostLikeData(content, memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求新增貼文點讚數資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
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