namespace DataInfo.Core.Models.Dto.Team.Request
{
    /// <summary>
    /// 踢離車隊隊員請求後端資料
    /// </summary>
    public class TeamKickRequest
    {
        /// <summary>
        /// Gets or sets 被踢離的車隊隊員 ID 列表
        /// </summary>
        public string KickIdList { get; set; }

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