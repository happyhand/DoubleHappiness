namespace DataInfo.Core.Models.Dto.Team.Request
{
    /// <summary>
    /// 更新隊員列表請求後端資料
    /// </summary>
    public class TeamUpdateMemberRequest
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
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }
}