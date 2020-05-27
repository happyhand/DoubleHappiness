namespace DataInfo.Core.Models.Dto.Team.Request
{
    /// <summary>
    /// 解散車隊請求後端資料
    /// </summary>
    public class TeamDisbandRequest
    {
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