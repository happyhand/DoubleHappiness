using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Enum;
using System;
using System.Linq;

namespace DataInfo.Service.Managers.Team
{
    /// <summary>
    /// 車隊基本服務
    /// </summary>
    public class TeamBaseService
    {
        /// <summary>
        /// 取得車隊互動狀態
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamDao">teamDao</param>
        /// <returns>TeamInteractiveType</returns>
        protected TeamInteractiveType GetTeamInteractiveStatus(string memberID, TeamDao teamDao)
        {
            if (teamDao.ApplyJoinList.Contains(memberID))
            {
                return TeamInteractiveType.ApplyJoin;
            }
            else if (teamDao.Leader.Equals(memberID) || teamDao.TeamViceLeaderIDs.Contains(memberID) || teamDao.TeamMemberIDs.Contains(memberID))
            {
                return TeamInteractiveType.Member;
            }
            else
            {
                return TeamInteractiveType.None;
            }
        }

        /// <summary>
        /// 會員是否已加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamDao">teamDao</param>
        /// <returns>bool</returns>
        protected bool MemberHasJoinTeam(string memberID, TeamDao teamDao)
        {
            return teamDao.Leader.Equals(memberID) || teamDao.TeamViceLeaderIDs.Contains(memberID) || teamDao.TeamMemberIDs.Contains(memberID);
        }

        /// <summary>
        /// 會員是否有車隊權限
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamDao">teamDao</param>
        /// <param name="isHighest">isHighest</param>
        /// <returns>bool</returns>
        protected bool MemberHasTeamAuthority(string memberID, TeamDao teamDao, bool isHighest)
        {
            return isHighest ? teamDao.Leader.Equals(memberID) : teamDao.Leader.Equals(memberID) || teamDao.TeamViceLeaderIDs.Contains(memberID);
        }

        /// <summary>
        /// 取得車隊角色
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamDao">teamDao</param>
        /// <returns>TeamRoleType</returns>
        protected TeamRoleType GetTeamRole(string memberID, TeamDao teamDao)
        {
            if (teamDao.Leader.Equals(memberID))
            {
                return TeamRoleType.Leader;
            }
            else if (teamDao.TeamViceLeaderIDs.Contains(memberID))
            {
                return TeamRoleType.ViceLeader;
            }
            else if (teamDao.TeamMemberIDs.Contains(memberID))
            {
                return TeamRoleType.Normal;
            }
            else
            {
                return TeamRoleType.None;
            }
        }
    }
}