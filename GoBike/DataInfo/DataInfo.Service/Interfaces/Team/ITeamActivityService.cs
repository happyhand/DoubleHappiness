﻿using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Enum;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Team
{
    /// <summary>
    /// 車隊活動服務
    /// </summary>
    public interface ITeamActivityService
    {
        /// <summary>
        /// 新增車隊活動
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Add(string memberID, TeamAddActivityContent content);

        /// <summary>
        /// 刪除車隊活動
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <param name="actID">actID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Delete(string memberID, string teamID, string actID);

        /// <summary>
        /// 更新車隊活動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Edit(string memberID, TeamUpdateActivityContent content);

        /// <summary>
        /// 取得車隊活動明細資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetDetail(string memberID, TeamActivityDetailContent content);

        /// <summary>
        /// 取得已參加的車隊活動列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetJoinList(string memberID);

        /// <summary>
        /// 取得車隊的活動列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetList(string memberID, TeamContent content);

        /// <summary>
        /// 加入或離開車隊活動
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="action">action</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> JoinOrLeave(TeamActivityContent content, string memberID, ActionType action);
    }
}