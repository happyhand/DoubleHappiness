namespace DataInfo.Core.Models.Dto.Team.Request
{
    /// <summary>
    /// 更新公告請求後端資料
    /// </summary>
    public class TeamUpdateBulletinRequest
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 公告 ID
        /// </summary>
        public string BulletinID { get; set; }

        /// <summary>
        /// Gets or sets 內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets 天數
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }
}