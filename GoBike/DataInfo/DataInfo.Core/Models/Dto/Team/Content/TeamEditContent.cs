namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 修改車隊資料內容
    /// </summary>
    public class TeamEditContent
    {
        /// <summary>
        /// Gets or sets 車隊頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 審查狀態
        /// </summary>
        public int ExamineStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊封面圖片路徑
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 搜尋狀態
        /// </summary>
        public int SearchStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }

        /// <summary>
        /// Gets or sets 車隊簡介
        /// </summary>
        public string TeamInfo { get; set; }
    }
}