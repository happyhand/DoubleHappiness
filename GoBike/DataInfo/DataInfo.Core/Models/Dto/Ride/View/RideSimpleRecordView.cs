using System;

namespace DataInfo.Core.Models.Dto.Ride.View
{
    /// <summary>
    /// 騎乘簡易記錄可視資料
    /// </summary>
    public class RideSimpleRecordView
    {
        /// <summary>
        /// Gets or sets 騎乘坡度
        /// </summary>
        public float Altitude { get; set; }

        /// <summary>
        /// Gets or sets 居住地
        /// </summary>
        public int County { get; set; }

        /// <summary>
        /// Gets or sets 建立日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets 騎乘距離
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// Gets or sets 等級
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets 騎乘 ID
        /// </summary>
        public string RideID { get; set; }

        /// <summary>
        /// Gets or sets 騎乘時間
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Gets or sets 標題
        /// </summary>
        public string Title { get; set; }
    }
}