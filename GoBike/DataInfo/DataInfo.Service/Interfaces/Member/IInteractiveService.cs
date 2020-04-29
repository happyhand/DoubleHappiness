﻿using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Response;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Member
{
    /// <summary>
    /// 互動服務
    /// </summary>
    public interface IInteractiveService
    {
        /// <summary>
        /// 刪除互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> DeleteInteractive(string memberID, InteractiveContent content);

        /// <summary>
        /// 取得黑名單列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetBlackList(string memberID);

        /// <summary>
        /// 取得好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetFriendList(string memberID);

        /// <summary>
        /// 更新互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <param name="status">status</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UpdateInteractive(string memberID, InteractiveContent content, int status);
    }
}