using System;

namespace DataInfo.Core.Models.Dto.Ride.View
{
    /// <summary>
    /// 組隊騎乘會員可視資料
    /// </summary>
    public class RideGroupMemberView
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 座標-X
        /// </summary>
        public string CoordinateX { get; set; }

        /// <summary>
        /// Gets or sets 座標-Y
        /// </summary>
        public string CoordinateY { get; set; }

        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }
    }
}