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
    /// 貼文功能
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Post")]
    public class PostController : JwtController
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("PostController");

        /// <summary>
        /// postService
        /// </summary>
        private readonly IPostService postService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="jwtService">jwtService</param>
        /// <param name="postService">postService</param>
        public PostController(IJwtService jwtService, IPostService postService) : base(jwtService)
        {
            this.postService = postService;
        }

        /// <summary>
        /// 貼文功能 - 刪除貼文資料
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

                ResponseResult responseResult = await this.postService.DeletePostData(memberID, postID).ConfigureAwait(false);
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
        /// 貼文功能 - 取得貼文資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Get(string? memberID)
        {
            string userID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求取得貼文資料", $"MemberID: {userID} TargetID: {memberID}", null);
                if (string.IsNullOrEmpty(memberID))
                {
                    return this.ResponseHandler(await this.postService.GetPostShowList(userID).ConfigureAwait(false));
                }

                return this.ResponseHandler(await this.postService.GetPostList(userID, memberID).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求取得貼文資料發生錯誤", $"MemberID: {userID} TargetID: {memberID}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 貼文功能 - 更新貼文資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPatch]
        public async Task<IActionResult> Patch(UpdatePostContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求更新貼文資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.postService.UpdatePostData(content, memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求更新貼文資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return this.ResponseHandler(new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                });
            }
        }

        /// <summary>
        /// 貼文功能 - 新增貼文資料
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Post(AddPostContent content)
        {
            string memberID = this.GetMemberID();
            try
            {
                this.logger.LogInfo("會員請求新增貼文資料", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                ResponseResult responseResult = await this.postService.AddPostData(content, memberID).ConfigureAwait(false);
                return this.ResponseHandler(responseResult);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求新增貼文資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
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