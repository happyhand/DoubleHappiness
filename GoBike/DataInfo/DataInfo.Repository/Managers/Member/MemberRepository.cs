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
        /// 取得指定會員資料
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="type">type</param>
        /// <returns>MemberDao</returns>
        public async Task<MemberDao> Get(string key, MemberSearchType type)
        {
            try
            {
                //// TODO 待確認是否要啟用 "是否可被搜尋" 功能
                ISugarQueryable<UserAccount, UserInfo> query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID));
                switch (type)
                {
                    case MemberSearchType.MemberID:
                        query = query.Where((ua, ui) => ua.MemberID.Equals(key));
                        break;

                    case MemberSearchType.Email:
                        query = query.Where((ua, ui) => ua.Email.Equals(key));
                        break;

                    case MemberSearchType.Nickname:
                        query = query.Where((ua, ui) => ui.NickName.Equals(key));
                        break;

                    case MemberSearchType.Mobile:
                        query = query.Where((ua, ui) => ui.Mobile.Equals(key));
                        break;

                    default:
                        this.logger.LogWarn("取得會員資料失敗，未設置搜尋類型", $"Key: {key} Type: {type}", null);
                        return null;
                }

                return (await this.TransformMemberDao(query).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤", $"Key: {key} Type: {type}", ex);
                return null;
            }
        }

        /// <summary>
        /// 搜詢會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <param name="ignoreMemberIDs">ignoreMemberIDs</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<MemberDao>> Search(string key, bool isFuzzy, IEnumerable<string> ignoreMemberIDs)
        {
            try
            {
                //// TODO 待確認是否要啟用 "是否可被搜尋" 功能
                //// 目前只開放 Email 跟 Nickname 提供搜尋
                ISugarQueryable<UserAccount, UserInfo> query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID));
                if (isFuzzy)
                {
                    query = query.Where((ua, ui) => ua.Email.Contains(key) || (!string.IsNullOrEmpty(ui.NickName) && ui.NickName.Contains(key)));
                }
                else
                {
                    query = query.Where((ua, ui) => ua.Email.Equals(key) || (!string.IsNullOrEmpty(ui.NickName) && ui.NickName.Equals(key)));
                }

                query = query.Where(ua => ignoreMemberIDs == null || !ignoreMemberIDs.Contains(ua.MemberID));
                return await this.TransformMemberDao(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜詢會員資料發生錯誤", $"Key: {key} IsFuzzy: {isFuzzy} IgnoreMemberIDs: {JsonConvert.SerializeObject(ignoreMemberIDs)}", ex);
                return new List<MemberDao>();
            }
        }

        /// <summary>
        /// 取得指定會員資料列表
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <param name="ignoreMemberIDs">ignoreMemberIDs</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<MemberDao>> Get(IEnumerable<string> memberIDs, IEnumerable<string> ignoreMemberIDs)
        {
            try
            {
                //// TODO 待確認是否要啟用 "是否可被搜尋" 功能
                ISugarQueryable<UserAccount, UserInfo> query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                           .Where(ua => memberIDs.Contains(ua.MemberID))
                           .Where(ua => ignoreMemberIDs == null || !ignoreMemberIDs.Contains(ua.MemberID));

                return await this.TransformMemberDao(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料列表發生錯誤", $"MemberIDs: {JsonConvert.SerializeObject(memberIDs)} IgnoreMemberIDs: {JsonConvert.SerializeObject(ignoreMemberIDs)}", ex);
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
            string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.LastLogin}";
            return await this.redisRepository.IsExist(cacheKey, false).ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
        }

        /// <summary>
        /// 轉換 MemberDao
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<MemberDao>> TransformMemberDao(ISugarQueryable<UserAccount, UserInfo> query)
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
    }
}