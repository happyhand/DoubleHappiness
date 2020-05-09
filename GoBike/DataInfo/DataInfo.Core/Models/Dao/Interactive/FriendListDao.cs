using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Interactive
{
    /// <summary>
    /// 好友列表資料
    /// </summary>
    public class FriendListDao
    {
        /// <summary>
        /// Gets or sets 好友列表
        /// </summary>
        public IEnumerable<string> FriendList { get; set; }
    }
}