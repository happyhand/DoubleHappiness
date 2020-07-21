using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Ride.Request
{
    /// <summary>
    /// 更新組隊騎乘請求後端資料
    /// </summary>
    public class UpdateRideGroupRequest
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 邀請列表
        /// </summary>
        public string InviteList { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }
    }
}