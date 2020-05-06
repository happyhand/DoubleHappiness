namespace DataInfo.Core.Models.Dto.Ride.Response
{
    /// <summary>
    /// 新增騎乘資訊後端回覆資料
    /// </summary>
    public class AddRideInfoResponse
    {
        /// <summary>
        /// Gets or sets 回覆結果
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Gets or sets 總高度
        /// </summary>
        public float TotalAltitude { get; set; }

        /// <summary>
        /// Gets or sets 總距離
        /// </summary>
        public float TotalDistance { get; set; }

        /// <summary>
        /// Gets or sets 總騎乘時間
        /// </summary>
        public long TotalRideTime { get; set; }
    }
}