using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Ride.Request
{
    /// <summary>
    /// 發送組隊騎乘通知請求後端資料
    /// </summary>
    public class RideGroupNotifyRequest
    {
        /// <summary>
        /// Gets or sets 請求動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }
    }
}