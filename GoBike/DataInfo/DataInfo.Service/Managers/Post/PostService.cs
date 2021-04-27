using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Post;
using DataInfo.Core.Models.Dto.Post.Content;
using DataInfo.Core.Models.Dto.Post.View;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Ride.Request;
using DataInfo.Core.Models.Dto.Ride.Response;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Repository.Interfaces.Post;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Post;
using DataInfo.Service.Interfaces.Server;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Post
{
    /// <summary>
    /// 貼文服務
    /// </summary>
    public class PostService : IPostService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("PostService");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        private readonly IMemberRepository memberRepository;

        /// <summary>
        /// postRepository
        /// </summary>
        private readonly IPostRepository postRepository;

        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// uploadService
        /// </summary>
        private readonly IUploadService uploadService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="uploadService">uploadService</param>
        /// <param name="serverService">serverService</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="postRepository">postRepository</param>
        public PostService(IMapper mapper, IUploadService uploadService, IServerService serverService, IMemberRepository memberRepository, IPostRepository postRepository)
        {
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.serverService = serverService;
            this.memberRepository = memberRepository;
            this.postRepository = postRepository;
        }

        /// <summary>
        /// 新增貼文資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> AddPostData(AddPostContent content, string memberID)
        {
            try
            {
                #region 上傳圖片

                if (content.Photo != null && content.Photo.Count() > 0)
                {
                    IEnumerable<string> imgUris = await this.uploadService.UploadRideImages(content.Photo, true).ConfigureAwait(false);
                    if (imgUris == null || !imgUris.Any())
                    {
                        this.logger.LogWarn("新增貼文資料失敗，上傳圖片失敗", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.UploadPhotoFail.ToString()
                        };
                    }

                    content.Photo = imgUris;
                }

                #endregion 上傳圖片

                #region 發送【建立貼文】指令至後端

                AddPostRequest request = this.mapper.Map<AddPostRequest>(content);
                request.MemberID = memberID;
                CommandData<AddPostResponse> response = await this.serverService.DoAction<AddPostResponse>((int)PostCommandIDType.CreateNewPost, CommandType.Post, request).ConfigureAwait(false);
                this.logger.LogInfo("新增貼文資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)CreateNewPostResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.CreateSuccess.ToString()
                        };

                    case (int)CreateNewPostResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.CreateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【建立貼文】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("新增貼文資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 新增貼文點讚數資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> AddPostLikeData(AddPostLikeContent content, string memberID)
        {
            try
            {
                if (string.IsNullOrEmpty(content.postID))
                {
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = ResponseErrorMessageType.PostIDEmpty.ToString()
                    };
                }

                #region 發送【新增點讚數】指令至後端

                AddPostLikeRequest request = this.mapper.Map<AddPostLikeRequest>(content);
                request.MemberID = memberID;
                CommandData<AddPostLikeResponse> response = await this.serverService.DoAction<AddPostLikeResponse>((int)PostCommandIDType.AddLike, CommandType.Post, request).ConfigureAwait(false);
                this.logger.LogInfo("新增貼文點讚數資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)AddPraiseResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)AddPraiseResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【新增點讚數】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("新增貼文點讚數資料結果發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 刪除貼文資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="postID">postID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> DeletePostData(string memberID, string postID)
        {
            try
            {
                #region 發送【刪除貼文】指令至後端

                DeletePostRequest request = new DeletePostRequest() { MemberID = memberID, PostID = postID };
                CommandData<DeletePostResponse> response = await this.serverService.DoAction<DeletePostResponse>((int)PostCommandIDType.DeletePost, CommandType.Post, request).ConfigureAwait(false);
                this.logger.LogInfo("刪除貼文資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)DeletePostResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)DeletePostResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【刪除貼文】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除貼文資料發生錯誤", $"MemberID: {memberID} PostID: {postID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 刪除貼文點讚數資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="postID">postID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> DeletePostLikeData(string memberID, string postID)
        {
            try
            {
                #region 發送【減少點讚數】指令至後端

                DeletePostLikeRequest request = new DeletePostLikeRequest() { MemberID = memberID, PostID = postID };
                request.MemberID = memberID;
                CommandData<DeletePostLikeResponse> response = await this.serverService.DoAction<DeletePostLikeResponse>((int)PostCommandIDType.ReduceLike, CommandType.Post, request).ConfigureAwait(false);
                this.logger.LogInfo("刪除貼文點讚數資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)ReducePraiseResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)ReducePraiseResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【減少點讚數】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除貼文點讚數資料結果發生錯誤", $"MemberID: {memberID} PostID: {postID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得會員的貼文列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="targetID">targetID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetPostList(string memberID, string targetID)
        {
            try
            {
                IEnumerable<PostInfoDao> postInfoDaos = await this.postRepository.GetMemberPostIDList(targetID).ConfigureAwait(false);
                IEnumerable<string> memberIDs = postInfoDaos.Select(dao => dao.MemberID);
                IEnumerable<MemberDao> memberDaos = await this.memberRepository.Get(memberIDs, null);
                Dictionary<string, MemberDao> memberDaoMap = memberDaos.ToDictionary(data => data.MemberID, data => data);
                IEnumerable<PostInfoView> views = postInfoDaos.Select(dao =>
                {
                    bool isGet = memberDaoMap.TryGetValue(dao.MemberID, out MemberDao memberDao);
                    if (!isGet)
                    {
                        this.logger.LogWarn("取得會員的貼文列表失敗，無會員資料", $"MemberID: {dao.MemberID}", null);
                        return null;
                    }

                    IEnumerable<string> likeList = JsonConvert.DeserializeObject<IEnumerable<string>>(dao.LikeList);
                    return new PostInfoView()
                    {
                        Avatar = Utility.GetMemberImageCdn(memberDao.Avatar),
                        Content = dao.Content,
                        CreateDate = dao.Content,
                        Like = likeList.Count(),
                        IsLike = likeList.Contains(memberID) ? 1 : 0,
                        MemberID = memberDao.MemberID,
                        Nickname = memberDao.Nickname,
                        Photo = JsonConvert.DeserializeObject<IEnumerable<string>>(dao.Photo).Select(img => Utility.GetRideImageCdn(img)),
                    };
                });

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = views.Where(view => view != null)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員的貼文列表發生錯誤", $"MemberID: {memberID} TargetID: {targetID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得會員的塗鴉牆顯示貼文列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetPostShowList(string memberID)
        {
            try
            {
                IEnumerable<PostInfoDao> postInfoDaos = await this.postRepository.GetMemberPostShowList(memberID).ConfigureAwait(false);
                IEnumerable<string> memberIDs = postInfoDaos.Select(dao => dao.MemberID);
                IEnumerable<MemberDao> memberDaos = await this.memberRepository.Get(memberIDs, null);
                Dictionary<string, MemberDao> memberDaoMap = memberDaos.ToDictionary(data => data.MemberID, data => data);
                IEnumerable<PostInfoView> views = postInfoDaos.Select(dao =>
                {
                    bool isGet = memberDaoMap.TryGetValue(dao.MemberID, out MemberDao memberDao);
                    if (!isGet)
                    {
                        this.logger.LogWarn("取得會員的塗鴉牆顯示貼文列表失敗，無會員資料", $"MemberID: {dao.MemberID}", null);
                        return null;
                    }

                    IEnumerable<string> likeList = JsonConvert.DeserializeObject<IEnumerable<string>>(dao.LikeList);
                    return new PostInfoView()
                    {
                        Avatar = Utility.GetMemberImageCdn(memberDao.Avatar),
                        Content = dao.Content,
                        CreateDate = dao.Content,
                        Like = likeList.Count(),
                        IsLike = likeList.Contains(memberID) ? 1 : 0,
                        MemberID = memberDao.MemberID,
                        Nickname = memberDao.Nickname,
                        Photo = JsonConvert.DeserializeObject<IEnumerable<string>>(dao.Photo).Select(img => Utility.GetRideImageCdn(img)),
                    };
                });

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = views.Where(view => view != null)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員的塗鴉牆顯示貼文列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 更新貼文資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdatePostData(UpdatePostContent content, string memberID)
        {
            try
            {
                #region 上傳圖片

                if (content.Photo != null && content.Photo.Count() > 0)
                {
                    IEnumerable<string> imgUris = await this.uploadService.UploadRideImages(content.Photo, true).ConfigureAwait(false);
                    if (imgUris == null || !imgUris.Any())
                    {
                        this.logger.LogWarn("更新貼文資料失敗，上傳圖片失敗", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.UploadPhotoFail.ToString()
                        };
                    }

                    content.Photo = imgUris;
                }

                #endregion 上傳圖片

                #region 發送【更新貼文】指令至後端

                UpdatePostRequest request = this.mapper.Map<UpdatePostRequest>(content);
                request.MemberID = memberID;
                CommandData<UpdatePostResponse> response = await this.serverService.DoAction<UpdatePostResponse>((int)PostCommandIDType.UpdatePost, CommandType.Post, request).ConfigureAwait(false);
                this.logger.LogInfo("更新貼文資料結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);

                switch (response.Data.Result)
                {
                    case (int)UpdatePostResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdateSuccess.ToString()
                        };

                    case (int)UpdatePostResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【更新貼文】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新貼文資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }
    }
}