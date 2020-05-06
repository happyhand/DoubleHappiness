using System;

namespace DataInfo.Core.Models.Dao.Ride
{
    /// <summary>
    /// 騎乘距離資料
    /// </summary>
    public class RideDistanceDao
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 總里程
        /// </summary>
        public float TotalDistance { get; set; }

        /// <summary>
        /// Gets or sets 週里程
        /// </summary>
        public float WeekDistance { get; set; }
    }
}