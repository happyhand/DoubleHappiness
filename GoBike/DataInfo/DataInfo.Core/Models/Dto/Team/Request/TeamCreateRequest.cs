namespace DataInfo.Core.Models.Dto.Team.Request
{
    /// <summary>
    /// 建立車隊請求後端資料
    /// </summary>
    public class TeamCreateRequest
    {
        /// <summary>
        /// Gets or sets 車隊頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets County
        /// </summary>
        public int County { get; set; }

        /// <summary>
        /// Gets or sets ExamineStatus
        /// </summary>
        public int ExamineStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊封面圖片路徑
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets SearchStatus
        /// </summary>
        public int SearchStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊簡介
        /// </summary>
        public string TeamInfo { get; set; }

        /// <summary>
        /// Gets or sets 車隊名稱
        /// </summary>
        public string TeamName { get; set; }
    }
}