using AutoMapper;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member.Table;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dao.Team.Table;
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Repository.Managers.Base;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers.Team
{
    /// <summary>
    /// 車隊活動資料庫
    /// </summary>
    public class TeamActivityRepository : MainBase, ITeamActivityRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamActivityRepository");

        /// <summary>
        /// 轉換 TeamActivityDao
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>TeamActivityDaos</returns>
        private async Task<IEnumerable<TeamActivityDao>> TransformTeamActivityDao(ISugarQueryable<TeamData, TeamActivity, UserInfo> query)
        {
            return await query.Select((td, ta, ui) =>
            new TeamActivityDao()
            {
                ActDate = ta.ActDate,
                ActID = ta.ActID,
                CreateDate = ta.CreateDate,
                FounderID = ui.MemberID,
                FounderName = ui.NickName,
                FounderAvatar = ui.Avatar,
                MaxAltitude = ta.MaxAltitude,
                MeetTime = ta.MeetTime,
                MemberListtDataJson = ta.MemberList,
                Route = ta.Route,
                TeamID = td.TeamID,
                TeamName = td.TeamName,
                Title = ta.Title,
                TotalDistance = ta.TotalDistance
            }).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 取得車隊活動資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamIDs">teamIDs</param>
        /// <returns>TeamActivityDaos</returns>
        public async Task<IEnumerable<TeamActivityDao>> Get(string memberID, string teamID)
        {
            try
            {
                ISugarQueryable<TeamData, TeamActivity, UserInfo> query = this.Db.Queryable<TeamData, TeamActivity, UserInfo>(
                                                          (td, ta, ui) => ta.TeamID.Equals(td.TeamID) && ta.MemberID.Equals(ui.MemberID))
                                                          .Where((td, ta, ui) => td.TeamID.Equals(teamID))
                                                          .Where((td, ta, ui) => td.Leader.Equals(memberID) || td.TeamViceLeaderIDs.Contains(memberID) || td.TeamMemberIDs.Contains(memberID));

                return (await this.TransformTeamActivityDao(query).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊活動資料列表發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
                return new List<TeamActivityDao>();
            }
        }

        /// <summary>
        /// 取得會員已加入的車隊活動資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamActivityDaos</returns>
        public async Task<IEnumerable<TeamActivityDao>> Get(string memberID)
        {
            try
            {
                ISugarQueryable<TeamData, TeamActivity, UserInfo> query = this.Db.Queryable<TeamData, TeamActivity, UserInfo>(
                                                          (td, ta, ui) => ta.TeamID.Equals(td.TeamID) && ta.MemberID.Equals(ui.MemberID))
                                                          .Where((td, ta, ui) => ta.MemberList.Contains(memberID));

                return (await this.TransformTeamActivityDao(query).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員已加入的車隊活動資料列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<TeamActivityDao>();
            }
        }

        /// <summary>
        /// 取得車隊活動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <param name="actID">actID</param>
        /// <returns>TeamActivityDao</returns>
        public async Task<TeamActivityDao> Get(string memberID, string teamID, string actID)
        {
            try
            {
                ISugarQueryable<TeamData, TeamActivity, UserInfo> query = this.Db.Queryable<TeamData, TeamActivity, UserInfo>(
                                                          (td, ta, ui) => ta.TeamID.Equals(td.TeamID) && ta.MemberID.Equals(ui.MemberID))
                                                          .Where((td, ta, ui) => ta.ActID.Equals(actID))
                                                          .Where((td, ta, ui) => td.TeamID.Equals(teamID))
                                                          .Where((td, ta) => td.Leader.Equals(memberID) || td.TeamViceLeaderIDs.Contains(memberID) || td.TeamMemberIDs.Contains(memberID));

                return (await this.TransformTeamActivityDao(query).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得指定車隊活動資料發生錯誤", $"MemberID: {memberID} ActID: {actID}", ex);
                return null;
            }
        }
    }
}