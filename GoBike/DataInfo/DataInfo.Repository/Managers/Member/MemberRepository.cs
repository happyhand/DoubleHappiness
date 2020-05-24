using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Member.Table;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers.Member
{
    /// <summary>
    /// 會員資料庫
    /// </summary>
    public class MemberRepository : MainBase, IMemberRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberRepository");

        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="redisRepository">redisRepository</param>
        public MemberRepository(IRedisRepository redisRepository)
        {
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 轉換 MemberDao
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>MemberDaos</returns>
        private async Task<IEnumerable<MemberDao>> TransformMemberDao(ISugarQueryable<UserAccount, UserInfo> query)
        {
            return await query.Select((ua, ui) =>
            new MemberDao()
            {
                Avatar = ui.Avatar,
                Birthday = ui.Birthday,
                BodyHeight = ui.BodyHeight,
                BodyWeight = ui.BodyWeight,
                County = ui.County,
                Email = ua.Email,
                FBToken = ua.FBToken,
                FrontCover = ui.FrontCover,
                Gender = ui.Gender,
                GoogleToken = ua.GoogleToken,
                MemberID = ua.MemberID,
                Mobile = ui.Mobile,
                Nickname = ui.NickName,
                RegisterDate = ua.RegisterDate,
                RegisterSource = ua.RegisterSource,
                Photo = ui.Photo,
                TeamListDataJson = ui.TeamList,
                FriendListDataJson = ui.FriendList,
                BlackListDataJson = ui.TeamList,
            }).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberDaos</returns>
        public async Task<MemberDao> Get(string memberID)
        {
            try
            {
                ISugarQueryable<UserAccount, UserInfo> query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                          .Where((ua, ui) => ua.MemberID.Equals(memberID));

                return (await this.TransformMemberDao(query).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤", $"MemberID: {memberID}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <param name="ignoreMemberIDs">ignoreMemberIDs</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<MemberDao>> Get(string searchKey, bool isFuzzy, IEnumerable<string> ignoreMemberIDs)
        {
            try
            {
                ISugarQueryable<UserAccount, UserInfo> query = null;
                if (isFuzzy)
                {
                    query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                          .Where((ua, ui) => ua.Email.Contains(searchKey) || (!string.IsNullOrEmpty(ui.NickName) && ui.NickName.Contains(searchKey)) || ua.MemberID.Contains(searchKey))
                          .Where(ua => ignoreMemberIDs == null || !ignoreMemberIDs.Contains(ua.MemberID));
                }
                else
                {
                    if (Utility.ValidateEmail(searchKey))
                    {
                        query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                          .Where((ua, ui) => ua.Email.Equals(searchKey))
                          .Where(ua => ignoreMemberIDs == null || !ignoreMemberIDs.Contains(ua.MemberID));
                    }
                    else if (searchKey.Contains(AppSettingHelper.Appsetting.MemberIDFlag))
                    {
                        query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                          .Where((ua, ui) => ua.MemberID.Equals(searchKey))
                          .Where(ua => ignoreMemberIDs == null || !ignoreMemberIDs.Contains(ua.MemberID));
                    }
                    else if (Utility.ValidateMobile(searchKey))
                    {
                        query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                          .Where((ua, ui) => !string.IsNullOrEmpty(ui.Mobile) && ui.Mobile.Equals(searchKey))
                          .Where(ua => ignoreMemberIDs == null || !ignoreMemberIDs.Contains(ua.MemberID));
                    }
                }

                if (query != null)
                {
                    return await this.TransformMemberDao(query).ConfigureAwait(false);
                }
                else
                {
                    return new List<MemberDao>();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤", $"SearchKey: {searchKey} IsFuzzy: {isFuzzy}", ex);
                return new List<MemberDao>();
            }
        }

        /// <summary>
        /// 取得會員資料列表
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <param name="ignoreMemberIDs">ignoreMemberIDs</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<MemberDao>> Get(IEnumerable<string> memberIDs, IEnumerable<string> ignoreMemberIDs)
        {
            try
            {
                ISugarQueryable<UserAccount, UserInfo> query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                           .Where(ua => memberIDs.Contains(ua.MemberID))
                           .Where(ua => ignoreMemberIDs == null || !ignoreMemberIDs.Contains(ua.MemberID));

                return await this.TransformMemberDao(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料列表發生錯誤", $"MemberIDs: {JsonConvert.SerializeObject(memberIDs)}", ex);
                return new List<MemberDao>();
            }
        }

        /// <summary>
        /// 取得會員的在線狀態
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>int</returns>
        public async Task<int> GetOnlineType(string memberID)
        {
            string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{AppSettingHelper.Appsetting.Redis.Flag.LastLogin}-{memberID}";
            return await this.redisRepository.IsExist(cacheKey).ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
        }
    }
}