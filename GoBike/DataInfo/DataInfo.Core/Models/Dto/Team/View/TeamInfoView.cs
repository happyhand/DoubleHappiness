using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Team.View
{
    /// <summary>
    /// 車隊資訊可視資料
    /// </summary>
    public class TeamInfoView
    {
        /// <summary>
        /// Gets or sets 車隊頭像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 互動狀態
        /// </summary>
        public int InteractiveStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊成員列表
        /// </summary>
        public IEnumerable<TeamMemberView> MemberList { get; set; }

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