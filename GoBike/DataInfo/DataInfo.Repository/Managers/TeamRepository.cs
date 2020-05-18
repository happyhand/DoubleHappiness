using AutoMapper;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dao.Team.Table;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers
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
        /// 取得車隊資料列表
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="type">type</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> Get(string searchKey, TeamSearchType type, bool isFuzzy)
        {
            try
            {
                IEnumerable<TeamData> teamDatas;
                switch (type)
                {
                    case TeamSearchType.TeamName:
                        //// 無法在 Where 裡作 ? 的判斷式，待以後有沒有其他解可優化
                        if (isFuzzy)
                        {
                            teamDatas = await this.Db.Queryable<TeamData>()
                                           .Where(data => data.TeamName.Contains(searchKey))
                                           .ToListAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            teamDatas = await this.Db.Queryable<TeamData>()
                                           .Where(data => data.TeamName.Equals(searchKey))
                                           .ToListAsync().ConfigureAwait(false);
                        }
                        break;

                    case TeamSearchType.TeamID:
                    default:
                        teamDatas = await this.Db.Queryable<TeamData>()
                                              .Where(data => data.TeamID.Equals(searchKey))
                                              .ToListAsync().ConfigureAwait(false);
                        break;
                }

                return this.mapper.Map<IEnumerable<TeamDao>>(teamDatas);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊資料列表發生錯誤", $"SearchKey: {searchKey} Type: {type} IsFuzzy: {isFuzzy}", ex);
                return new List<TeamDao>();
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
    }
}