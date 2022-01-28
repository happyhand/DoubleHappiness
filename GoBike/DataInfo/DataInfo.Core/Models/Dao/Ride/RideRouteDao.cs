using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Ride
{
    /// <summary>
    /// 組隊騎乘路線資料
    /// </summary>
    public class RideRouteDao
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 該筆紀錄的索引總數
        /// </summary>
        public int IndexCount { get; set; }
        
        /// <summary>
        /// Gets or sets 該筆紀錄的索引值
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets 騎乘路線
        /// </summary>
        public IEnumerable<IEnumerable<string>> Route { get; set; }
    }
}