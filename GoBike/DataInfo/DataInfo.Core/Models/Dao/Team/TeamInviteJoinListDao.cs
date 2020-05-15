using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Team
{
    /// <summary>
    /// 車隊邀請列表資料
    /// </summary>
    public class TeamInviteJoinListDao
    {
        /// <summary>
        /// Gets or sets 邀請列表
        /// </summary>
        public IEnumerable<string> Invite { get; set; }
    }
}