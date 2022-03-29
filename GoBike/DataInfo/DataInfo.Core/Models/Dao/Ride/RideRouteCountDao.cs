using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Ride
{
    /// <summary>
    /// 組隊騎乘路線索引資料
    /// </summary>
    public class RideRouteCountDao
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets RideID
        /// </summary>
        public string RideID { get; set; }

        /// <summary>
        /// Gets or sets 該筆紀錄的索引總數
        /// </summary>
        public int IndexCount { get; set; }
    }
}