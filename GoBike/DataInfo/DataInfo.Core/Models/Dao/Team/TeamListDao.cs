using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Team
{
    /// <summary>
    /// 車隊列表資料
    /// </summary>
    public class TeamListDao
    {
        /// <summary>
        /// Gets or sets 車隊列表
        /// </summary>
        public IEnumerable<string> TeamList { get; set; }
    }
}