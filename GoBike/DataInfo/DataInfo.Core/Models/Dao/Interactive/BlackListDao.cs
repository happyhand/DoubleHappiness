using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Interactive
{
    /// <summary>
    /// 黑名單列表資料
    /// </summary>
    public class BlackListDao
    {
        /// <summary>
        /// Gets or sets 黑名單列表
        /// </summary>
        public IEnumerable<string> BlackList { get; set; }
    }
}