using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Ride
{
    /// <summary>
    /// 組隊騎乘路線資訊資料
    /// </summary>
    public class RideRouteInfoDao
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 該筆紀錄的索引值
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets 騎乘路線
        /// </summary>
        public string Route { get; set; }
    }
}