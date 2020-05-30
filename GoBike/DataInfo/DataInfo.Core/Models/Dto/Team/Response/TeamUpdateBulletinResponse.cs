namespace DataInfo.Core.Models.Dto.Team.Response
{
    /// <summary>
    /// 更新公告後端回覆資料
    /// </summary>
    public class TeamUpdateBulletinResponse
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
        /// Gets or sets 回覆結果
        /// </summary>
        public int Result { get; set; }
    }
}