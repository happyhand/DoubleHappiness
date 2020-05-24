namespace DataInfo.Core.Models.Dto.Team.Request
{
    /// <summary>
    /// 加入或離開車隊活動請求後端資料
    /// </summary>
    public class TeamJoinOrLeaveActivityRequest
    {
        /// <summary>
        /// Gets or sets 活動 ID
        /// </summary>
        public string ActID { get; set; }

        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 發起人會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }
}