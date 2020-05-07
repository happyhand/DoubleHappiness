namespace DataInfo.Core.Models.Dto.Interactive.Request
{
    /// <summary>
    /// 更新黑名單列表請求後端資料
    /// </summary>
    public class UpdateBlackListRequest
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 黑名單 ID
        /// </summary>
        public string BlackID { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }
    }
}