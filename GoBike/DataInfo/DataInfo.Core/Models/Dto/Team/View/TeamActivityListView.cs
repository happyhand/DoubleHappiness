using DataInfo.Core.Models.Dto.Team.View.data;
using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Team.View
{
    /// <summary>
    /// 車隊活動列表可視資料
    /// </summary>
    public class TeamActivityListView
    {
        /// <summary>
        /// Gets or sets 活動日期
        /// </summary>
        public string ActDate { get; set; }

        /// <summary>
        /// Gets or sets 活動 ID
        /// </summary>
        public string ActID { get; set; }

        /// <summary>
        /// Gets or sets 主辦人頭像
        /// </summary>
        public string FounderAvatar { get; set; }

        /// <summary>
        /// Gets or sets 主辦人名稱
        /// </summary>
        public string FounderName { get; set; }

        /// <summary>
        /// Gets or sets 是否已經參加
        /// </summary>
        public int HasJoin { get; set; }

        /// <summary>
        /// Gets or sets 道路線圖
        /// </summary>
        public IEnumerable<IEnumerable<LoadMapView>> LoadMap { get; set; }

        /// <summary>
        /// Gets or sets 最高海拔
        /// </summary>
        public float MaxAltitude { get; set; }

        /// <summary>
        /// Gets or sets 集合時間
        /// </summary>
        public string MeetTime { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }

        /// <summary>
        /// Gets or sets 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets 總距離
        /// </summary>
        public IEnumerable<float> TotalDistance { get; set; }
    }
}