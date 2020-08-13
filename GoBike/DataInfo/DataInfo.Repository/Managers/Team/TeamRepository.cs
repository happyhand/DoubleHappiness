using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Member.Table;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dao.Team.Table;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers.Team
{
    /// <summary>
    /// 車隊資料庫
    /// </summary>
    public class TeamRepository : MainBase, ITeamRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamRepository");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        private readonly IMemberRepository memberRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="memberRepository">memberRepository</param>
        public TeamRepository(IMapper mapper, IMemberRepository memberRepository)
        {
            this.mapper = mapper;
            this.memberRepository = memberRepository;
        }

        /// <summary>
        /// 取得車隊資料列表
        /// </summary>
        /// <param name="teamIDs">teamIDs</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> Get(IEnumerable<string> teamIDs)
        {
            try
            {
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => teamIDs.Contains(data.TeamID))
                                              .ToListAsync().ConfigureAwait(false);

                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊資料列表發生錯誤", $"TeamIDs: {JsonConvert.SerializeObject(teamIDs)}", ex);
                return new List<TeamDao>();
            }
        }

        /// <summary>
        /// 取得車隊資料
        /// </summary>
        /// <param name="teamID">teamID</param>
        /// <returns>TeamDao</returns>
        public async Task<TeamDao> Get(string teamID)
        {
            try
            {
                TeamData teamData = await this.Db.Queryable<TeamData>()
                                              .Where(data => data.TeamID.Equals(teamID))
                                              .FirstAsync().ConfigureAwait(false);

                return this.mapper.Map<TeamDao>(teamData);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊資料發生錯誤", $"TeamID: {teamID}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得車隊成員列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<MemberDao>> GetMemberList(string memberID, string teamID)
        {
            try
            {
                ISugarQueryable<UserAccount, UserInfo> query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                        .Where((ua, ui) => SqlFunc.Subqueryable<TeamData>()
                        .Where(td => td.TeamID.Equals(teamID))
                        .Where(td => td.Leader.Equals(memberID) || td.TeamViceLeaderIDs.Contains(memberID) || td.TeamMemberIDs.Contains(memberID))
                        .Where(td => td.Leader.Equals(ua.MemberID) || td.TeamViceLeaderIDs.Contains(ua.MemberID) || td.TeamMemberIDs.Contains(ua.MemberID))
                        .Any());

                return await this.memberRepository.TransformMemberDao(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊成員列表發生錯誤", $"TeamID: {teamID}", ex);
                return new List<MemberDao>();
            }
        }

        /// <summary>
        /// 取得申請加入會員列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>MemberDaos</returns>
        public async Task<IEnumerable<MemberDao>> GetMemberOfApplyJoin(string memberID, string teamID)
        {
            try
            {
                ISugarQueryable<UserAccount, UserInfo> query = this.Db.Queryable<UserAccount, UserInfo>((ua, ui) => ua.MemberID.Equals(ui.MemberID))
                        .Where((ua, ui) => SqlFunc.Subqueryable<TeamData>()
                        .Where(td => td.TeamID.Equals(teamID))
                        .Where(td => td.Leader.Equals(memberID) || td.TeamViceLeaderIDs.Contains(memberID))
                        .Where(td => td.ApplyJoinList.Contains(ua.MemberID))
                        .Any());

                return await this.memberRepository.TransformMemberDao(query).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得申請加入會員列表發生錯誤", $"TeamID: {teamID}", ex);
                return new List<MemberDao>();
            }
        }

        /// <summary>
        /// 取得附近車隊資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="county">county</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetNearbyTeam(string memberID, int county)
        {
            try
            {
                int takeBrowseCount = AppSettingHelper.Appsetting.Rule.TakeBrowseCount;
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => data.County.Equals(county))
                                              .Where(data => !data.Leader.Equals(memberID))
                                              .Where(data => !data.TeamViceLeaderIDs.Contains(memberID))
                                              .Where(data => !data.TeamMemberIDs.Contains(memberID))
                                              .Take(takeBrowseCount)
                                              .ToListAsync().ConfigureAwait(false);

                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得附近車隊資料列表發生錯誤", $"County: {county}", ex);
                return new List<TeamDao>();
            }
        }

        /// <summary>
        /// 取得新創車隊資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetNewCreationTeam(string memberID)
        {
            int daysOfNewCreation = AppSettingHelper.Appsetting.Rule.DaysOfNewCreation;
            int takeBrowseCount = AppSettingHelper.Appsetting.Rule.TakeBrowseCount;
            DateTime expiredDate = DateTime.UtcNow.AddDays(daysOfNewCreation * -1);
            try
            {
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => Convert.ToDateTime(data).Ticks - expiredDate.Ticks > 0)
                                              .Where(data => !data.Leader.Equals(memberID))
                                              .Where(data => !data.TeamViceLeaderIDs.Contains(memberID))
                                              .Where(data => !data.TeamMemberIDs.Contains(memberID))
                                              .Take(takeBrowseCount)
                                              .ToListAsync().ConfigureAwait(false);

                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得新創車隊資料列表發生錯誤", $"ExpiredDate: {expiredDate}", ex);
                return new List<TeamDao>();
            }
        }

        /// <summary>
        /// 取得推薦車隊資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetRecommendTeam(string memberID)
        {
            try
            {
                int takeBrowseCount = AppSettingHelper.Appsetting.Rule.TakeBrowseCount;
                //// TODO 待確認推薦標準
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              //.Where(data => (data.TeamViceLeaderIDs.Count() + data.TeamMemberIDs.Count()) >= 50) //// 無法使用 JSON ... 等其他 Function
                                              .Where(data => !data.Leader.Equals(memberID))
                                              .Where(data => !data.TeamViceLeaderIDs.Contains(memberID))
                                              .Where(data => !data.TeamMemberIDs.Contains(memberID))
                                              .Take(takeBrowseCount)
                                              .ToListAsync().ConfigureAwait(false);

                teamDatas = teamDatas.Where(data => (JsonConvert.DeserializeObject<IEnumerable<string>>(data.TeamViceLeaderIDs).Count()
                                                    + JsonConvert.DeserializeObject<IEnumerable<string>>(data.TeamMemberIDs).Count())
                                                    >= 50);
                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得推薦車隊資料列表發生錯誤", string.Empty, ex);
                return new List<TeamDao>();
            }
        }

        /// <summary>
        /// 取得會員的車隊資料列表
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetTeamListOfMember(string memberID)
        {
            try
            {
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<UserInfo, TeamData>((ui, td) => ui.TeamList.Contains(td.TeamID))
                                                               .Where(ui => ui.MemberID.Equals(memberID))
                                                               .Select((ui, td) => td)
                                                               .ToListAsync().ConfigureAwait(false);
                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員的車隊資料列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<TeamDao>();
            }
        }

        /// <summary>
        /// 取得申請加入車隊列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetTeamOfApplyJoin(string memberID)
        {
            try
            {
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => data.ApplyJoinList.Contains(memberID))
                                              .ToListAsync().ConfigureAwait(false);

                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得申請加入車隊列表發生錯誤", $"MemberID: {JsonConvert.SerializeObject(memberID)}", ex);
                return new List<TeamDao>();
            }
        }

        /// <summary>
        /// 搜尋車隊
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> Search(string key)
        {
            try
            {
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                           .Where(data => data.TeamName.Contains(key))
                                           .ToListAsync().ConfigureAwait(false);

                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋車隊發生錯誤", $"Key: {key}", ex);
                return new List<TeamDao>();
            }
        }
    }
}