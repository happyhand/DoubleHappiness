using AutoMapper;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member.Table;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dao.Team.Table;
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers.Team
{
    /// <summary>
    /// 車隊公告資料庫
    /// </summary>
    public class TeamBulletinRepository : MainBase, ITeamBulletinRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamBulletinRepository");

        /// <summary>
        /// 轉換 TeamBulletinDao
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>TeamBulletinDaos</returns>
        private async Task<IEnumerable<TeamBulletinDao>> TransformTeamBulletinDao(ISugarQueryable<TeamData, TeamBulletin, UserInfo> query)
        {
            return await query.Select((td, tb, ui) =>
            new TeamBulletinDao()
            {
                Avatar = ui.Avatar,
                BulletinID = tb.BulletinID,
                Content = tb.Content,
                CreateDate = tb.CreateDate,
                Day = tb.Day,
                MemberID = ui.MemberID,
                Nickname = ui.NickName,
                TeamID = td.TeamID
            }).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 取得車隊公告資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>TeamBulletinDaos</returns>
        public async Task<IEnumerable<TeamBulletinDao>> Get(string memberID, string teamID)
        {
            try
            {
                using (SqlSugarClient db = this.NewDB)
                {
                    ISugarQueryable<TeamData, TeamBulletin, UserInfo> query = db.Queryable<TeamData, TeamBulletin, UserInfo>(
                                                           (td, tb, ui) => tb.TeamID.Equals(teamID) && tb.TeamID.Equals(td.TeamID) && tb.MemberID.Equals(ui.MemberID))
                                                           .Where((td, tb, ui) => td.Leader.Equals(memberID) || td.TeamViceLeaderIDs.Contains(memberID) || td.TeamMemberIDs.Contains(memberID));

                    return (await this.TransformTeamBulletinDao(query).ConfigureAwait(false));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊公告資料列表發生錯誤", $"MemberID: {memberID} TeamID: {teamID}", ex);
                return new List<TeamBulletinDao>();
            }
        }
    }
}