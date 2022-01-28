using System;

namespace DataInfo.Core.Models.Dao.Ride
{
    /// <summary>
    /// 騎乘資料
    /// </summary>
    public class RideDao
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
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 封面圖片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets 騎乘 ID
        /// </summary>
        public string RideID { get; set; }

        /// <summary>
        /// Gets or sets 分享內容
        /// </summary>
        public string ShareContent { get; set; }

        /// <summary>
        /// Gets or sets 分享類型
        /// </summary>
        public int SharedType { get; set; }

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