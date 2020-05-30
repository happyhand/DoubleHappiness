namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 更新車隊公告內容
    /// </summary>
    public class TeamUpdateBulletinContent
    {
        /// <summary>
        /// Gets or sets 公告 ID
        /// </summary>
        public string BulletinID { get; set; }

        /// <summary>
        /// Gets or sets 內容
        /// </summary>
        public string Content { get; set; }
    }
}