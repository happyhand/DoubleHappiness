namespace DataInfo.Core.Models.Dto.Ride.Request
{
    /// <summary>
    /// 更新組隊騎乘邀請請求後端資料
    /// </summary>
    public class UpdateInviteListRequest
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 更新列表
        /// </summary>
        public string UpdateList { get; set; }
    }
}