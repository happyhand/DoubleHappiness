namespace DataInfo.Core.Models.Dao.Ride.Table
{
    /// <summary>
    /// 騎乘週資料
    /// </summary>
    public class WeekRideData
    {
        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets WeekDistance
        /// </summary>
        public float WeekDistance { get; set; }

        /// <summary>
        /// Gets or sets WeekFirstDay
        /// </summary>
        public string WeekFirstDay { get; set; }

        /// <summary>
        /// Gets or sets WeekLastDay
        /// </summary>
        public string WeekLastDay { get; set; }
    }
}