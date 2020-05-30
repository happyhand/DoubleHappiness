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
        /// Gets or sets 最高海拔
        /// </summary>
        public float MaxAltitude { get; set; }

        /// <summary>
        /// Gets or sets 集合時間
        /// </summary>
        public string MeetTime { get; set; }

        /// <summary>
        /// Gets or sets 活動圖片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets 總距離
        /// </summary>
        public float TotalDistance { get; set; }

        //// TODO 路線、路線描述
    }
}