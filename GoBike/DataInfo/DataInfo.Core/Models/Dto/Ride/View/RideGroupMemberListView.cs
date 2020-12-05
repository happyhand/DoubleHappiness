using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Ride.View
{
    /// <summary>
    /// 組隊騎乘成員列表可視資料
    /// </summary>
    public class RideGroupMemberListView
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public RideGroupMemberView Leader { get; set; }

        /// <summary>
        /// Gets or sets 座標-X
        /// </summary>
        public IEnumerable<RideGroupMemberView> MemberList { get; set; }
    }
}