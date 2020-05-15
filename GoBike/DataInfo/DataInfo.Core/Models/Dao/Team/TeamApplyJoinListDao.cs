using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Team
{
    /// <summary>
    /// 車隊申請列表資料
    /// </summary>
    public class TeamApplyJoinListDao
    {
        /// <summary>
        /// Gets or sets 申請列表
        /// </summary>
        public IEnumerable<string> Apply { get; set; }
    }
}