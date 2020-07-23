namespace DataInfo.Core.Models.Dto.Ride.Request
{
    /// <summary>
    /// 更新組隊騎乘座標請求後端資料
    /// </summary>
    public class UpdateRideGroupCoordinateRequest
    {
        /// <summary>
        /// Gets or sets 座標 X
        /// </summary>
        public string CoordinateX { get; set; }

        /// <summary>
        /// Gets or sets 座標 Y
        /// </summary>
        public string CoordinateY { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }
    }
}