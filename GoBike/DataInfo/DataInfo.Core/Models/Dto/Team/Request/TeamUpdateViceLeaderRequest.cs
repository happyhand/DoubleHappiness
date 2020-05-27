namespace DataInfo.Core.Models.Dto.Team.Request
{
    /// <summary>
    /// 更新車隊副隊長請求後端資料
    /// </summary>
    public class TeamUpdateViceLeaderRequest
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 車隊隊長 ID
        /// </summary>
        public string LeaderID { get; set; }

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