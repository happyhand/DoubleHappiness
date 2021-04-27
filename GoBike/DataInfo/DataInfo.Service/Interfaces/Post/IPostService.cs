using DataInfo.Core.Models.Dto.Post.Content;
using DataInfo.Core.Models.Dto.Response;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Post
{
    /// <summary>
    /// 貼文服務
    /// </summary>
    public interface IPostService
    {
        /// <summary>
        /// 新增貼文資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> AddPostData(AddPostContent content, string memberID);

        /// <summary>
        /// 新增貼文點讚數資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> AddPostLikeData(AddPostLikeContent content, string memberID);

        /// <summary>
        /// 刪除貼文資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="postID">postID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> DeletePostData(string memberID, string postID);

        /// <summary>
        /// 刪除貼文點讚數資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="postID">postID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> DeletePostLikeData(string memberID, string postID);

        /// <summary>
        /// 取得會員的貼文列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="targetID">targetID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetPostList(string memberID, string targetID);

        /// <summary>
        /// 取得會員的塗鴉牆顯示貼文列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetPostShowList(string memberID);

        /// <summary>
        /// 更新貼文資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UpdatePostData(UpdatePostContent content, string memberID);
    }
}