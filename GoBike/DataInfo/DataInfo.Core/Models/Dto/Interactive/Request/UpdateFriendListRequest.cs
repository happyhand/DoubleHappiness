namespace DataInfo.Core.Models.Dto.Interactive.Request
{
    /// <summary>
    /// 更新好友列表請求後端資料
    /// </summary>
    public class UpdateFriendListRequest
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 朋友 ID
        /// </summary>
        public string FriendID { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }
    }
}