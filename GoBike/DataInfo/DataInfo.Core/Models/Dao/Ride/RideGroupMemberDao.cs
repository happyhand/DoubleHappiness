using System;

namespace DataInfo.Core.Models.Dao.Ride
{
    /// <summary>
    /// 組隊騎乘會員資料
    /// </summary>
    public class RideGroupMemberDao
    {
        /// <summary>
        /// Gets or sets 座標-X
        /// </summary>
        public string CoordinateX { get; set; }

        /// <summary>
        /// Gets or sets 座標-Y
        /// </summary>
        public string CoordinateY { get; set; }

        /// <summary>
        /// Gets or sets 組隊騎乘 Redis Key
        /// </summary>
        public string RideGroupKey { get; set; }
    }
}