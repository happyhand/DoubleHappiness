namespace DataInfo.Core.Models.Dto.Team.View
{
    /// <summary>
    /// 車隊成員可視資料
    /// </summary>
    public class TeamMemberView
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets 是否在線
        /// </summary>
        public int OnlineType { get; set; }

        /// <summary>
        /// Gets or sets 角色
        /// </summary>
        public int Role { get; set; }
    }
}