using DataInfo.Core.Models.Dto.Team.Content.data;
using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 更新車隊活動內容
    /// </summary>
    public class TeamUpdateActivityContent
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
        /// Gets or sets 道路線圖資料
        /// </summary>
        public IEnumerable<IEnumerable<LoadMap>> LoadMap { get; set; }

        /// <summary>
        /// Gets or sets 最高海拔
        /// </summary>
        public float MaxAltitude { get; set; }

        /// <summary>
        /// Gets or sets 集合時間
        /// </summary>
        public string MeetTime { get; set; }

        /// <summary>
        /// Gets or sets 路線
        /// </summary>
        public IEnumerable<Route> Routes { get; set; }

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