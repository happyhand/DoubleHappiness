namespace DataInfo.Core.Models.Dto.Ride.Request
{
    /// <summary>
    /// 回覆組隊騎乘請求後端資料
    /// </summary>
    public class ReplyRideGroupRequest
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }
    }
}