using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Interactive;
using DataInfo.Core.Models.Dao.Member.Table;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers
{
    /// <summary>
    /// 互動資料庫
    /// </summary>
    public class InteractiveRepository : MainBase, IInteractiveRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("InteractiveRepository");

        /// <summary>
        /// 取得被加入好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<string>> GetBeFriendList(string memberID)
        {
            try
            {
                return await this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => new object[] {
                        JoinType.Left,ua.MemberID.Equals(ui.MemberID)})
                         .Where((ua, ui) => !ua.MemberID.Equals(memberID))
                         .Where((ua, ui) => ui.FriendList.Contains(memberID))
                         .Select(ua => ua.MemberID).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得被加入好友列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// 取得黑名單列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<string>> GetBlackList(string memberID)
        {
            try
            {
                string blackListDataJson = await this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => new object[] {
                        JoinType.Left,ua.MemberID.Equals(ui.MemberID)})
                         .Where((ua, ui) => ua.MemberID.Equals(memberID))
                         .Select((ua, ui) => ui.BlackList).FirstAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<string>>(blackListDataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得黑名單列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// 取得好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<string>> GetFriendList(string memberID)
        {
            try
            {
                string friendListDataJson = await this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => new object[] {
                        JoinType.Left,ua.MemberID.Equals(ui.MemberID)})
                         .Where((ua, ui) => ua.MemberID.Equals(memberID))
                         .Select((ua, ui) => ui.FriendList).FirstAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<string>>(friendListDataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得好友列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<string>();
            }
        }
    }
}