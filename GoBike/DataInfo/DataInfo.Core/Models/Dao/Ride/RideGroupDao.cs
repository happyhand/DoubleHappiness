using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Ride
{
    /// <summary>
    /// 組隊騎乘資料
    /// </summary>
    public class RideGroupDao
    {
        /// <summary>
        /// Gets or sets 邀請列表
        /// </summary>
        public IEnumerable<string> InviteList { get; set; }

        /// <summary>
        /// Gets or sets 發起人 ID
        /// </summary>
        public string Leader { get; set; }

        /// <summary>
        /// Gets or sets 組隊成員列表
        /// </summary>
        public IEnumerable<string> MemberList { get; set; }
    }
}