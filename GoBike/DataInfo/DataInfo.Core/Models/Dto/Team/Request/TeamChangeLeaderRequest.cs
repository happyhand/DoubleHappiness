namespace DataInfo.Core.Models.Dto.Team.Request
{
    /// <summary>
    /// 更換車隊隊長請求後端資料
    /// </summary>
    public class TeamChangeLeaderRequest
    {
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