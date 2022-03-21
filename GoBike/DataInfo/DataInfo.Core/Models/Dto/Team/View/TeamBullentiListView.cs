namespace DataInfo.Core.Models.Dto.Team.View
{
    /// <summary>
    /// 車隊公告列表可視資料
    /// </summary>
    public class TeamBullentiListView
    {
        /// <summary>
        /// Gets or sets 公告人頭像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 公告 ID
        /// </summary>
        public string BulletinID { get; set; }

        /// <summary>
        /// Gets or sets 內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets 建立日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets 天數
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Gets or sets 公告人名稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }
}