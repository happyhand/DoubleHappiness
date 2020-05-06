namespace DataInfo.Core.Models.Dao.Ride.Table
{
    /// <summary>
    /// 騎乘資料
    /// </summary>
    public class RideData
    {
        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets TotalAltitude
        /// </summary>
        public float TotalAltitude { get; set; }

        /// <summary>
        /// Gets or sets TotalDistance
        /// </summary>
        public float TotalDistance { get; set; }

        /// <summary>
        /// Gets or sets TotalRideTime
        /// </summary>
        public long TotalRideTime { get; set; }
    }
}