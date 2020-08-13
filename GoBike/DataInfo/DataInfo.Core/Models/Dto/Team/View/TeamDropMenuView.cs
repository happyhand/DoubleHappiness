using System;

namespace DataInfo.Core.Models.Dto.Team.View
{
    /// <summary>
    /// 車隊下拉選單可視資料
    /// </summary>
    public class TeamDropMenuView
    {
        /// <summary>
        /// Gets or sets 車隊頭像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 訊息最新時間
        /// </summary>
        public DateTime MessageLatestTime { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }

        /// <summary>
        /// Gets or sets 車隊名稱
        /// </summary>
        public string TeamName { get; set; }
    }
}