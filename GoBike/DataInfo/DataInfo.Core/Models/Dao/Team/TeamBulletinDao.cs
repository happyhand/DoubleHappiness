namespace DataInfo.Core.Models.Dao.Team
{
    /// <summary>
    /// 車隊公告資料
    /// </summary>
    public class TeamBulletinDao
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
        /// Gets or sets 公告人 ID
        /// </summary>
        public string MemberID { get; set; }

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