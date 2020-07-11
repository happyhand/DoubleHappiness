namespace DataInfo.Core.Models.Dto.Team.View
{
    /// <summary>
    /// 車隊設定可視資料
    /// </summary>
    public class TeamSettingView
    {
        /// <summary>
        /// Gets or sets 車隊頭像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 車隊所在地
        /// </summary>
        public int County { get; set; }

        /// <summary>
        /// Gets or sets 審核狀態(1:開，-1:關)
        /// </summary>
        public int ExamineStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊封面
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 搜尋狀態(1:開，-1:關)
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