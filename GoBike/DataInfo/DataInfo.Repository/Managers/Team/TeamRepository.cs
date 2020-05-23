using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dao.Team.Table;
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
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
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        public TeamRepository(IMapper mapper)
        {
            this.mapper = mapper;
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
        /// 取得申請車隊列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetApplyTeamList(string memberID)
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
                this.logger.LogError("取得申請車隊列表發生錯誤", $"MemberID: {JsonConvert.SerializeObject(memberID)}", ex);
                return new List<TeamDao>();
            }
        }

        /// <summary>
        /// 取得邀請車隊列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetInviteTeamList(string memberID)
        {
            try
            {
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => data.InviteJoinList.Contains(memberID))
                                              .ToListAsync().ConfigureAwait(false);

                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得邀請車隊列表發生錯誤", $"MemberID: {JsonConvert.SerializeObject(memberID)}", ex);
                return new List<TeamDao>();
            }
        }

        /// <summary>
        /// 取得附近車隊資料列表
        /// </summary>
        /// <param name="county">county</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetNearbyTeam(int county)
        {
            try
            {
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => data.County.Equals(county))
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
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetNewCreationTeam()
        {
            DateTime expiredDate = DateTime.UtcNow.AddDays(AppSettingHelper.Appsetting.DaysOfNewCreation * -1);
            try
            {
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => Convert.ToDateTime(data).Ticks - expiredDate.Ticks > 0)
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
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> GetRecommendTeam()
        {
            try
            {
                //// TODO 待確認推薦標準
                IEnumerable<TeamData> teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => data.TeamViceLeaderIDs.Length + data.TeamMemberIDs.Length >= 50)
                                              .ToListAsync().ConfigureAwait(false);

                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得推薦車隊資料列表發生錯誤", string.Empty, ex);
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