using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Team
{
    /// <summary>
    /// 車隊隊員列表資料
    /// </summary>
    public class TeamMemberIDListDao
    {
        /// <summary>
        /// Gets or sets 車隊隊員列表
        /// </summary>
        public IEnumerable<string> Member { get; set; }
    }
}