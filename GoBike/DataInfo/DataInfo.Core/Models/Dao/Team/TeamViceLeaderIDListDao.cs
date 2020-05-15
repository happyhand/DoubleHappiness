using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Team
{
    /// <summary>
    /// 車隊副隊長列表資料
    /// </summary>
    public class TeamViceLeaderIDListDao
    {
        /// <summary>
        /// Gets or sets 車隊副隊長列表
        /// </summary>
        public IEnumerable<string> ViceLeader { get; set; }
    }
}