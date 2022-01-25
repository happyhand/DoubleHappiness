namespace DataInfo.Core.Models.Dto.Ride.Request
{
    /// <summary>
    /// 新增騎乘路線資訊請求後端資料
    /// </summary>
    public class AddRideRouteRequest
    {
        /// <summary>
        /// Gets or sets 騎乘 ID
        /// </summary>
        public string RideID { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }
        /// <summary>
        /// Gets or sets 路線資訊
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets 索引總數
        /// </summary>
        public int IndexCount { get; set; }

        /// <summary>
        /// Gets or sets 該筆紀錄的索引值
        /// </summary>
        public int Index { get; set; }
    }
}