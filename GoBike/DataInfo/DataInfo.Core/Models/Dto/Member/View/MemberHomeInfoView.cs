namespace DataInfo.Core.Models.Dto.Member.View
{
    /// <summary>
    /// 會員首頁資訊可視資料
    /// </summary>
    public class MemberHomeInfoView
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets 首頁圖片路徑
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets 總里程
        /// </summary>
        public float TotalDistance { get; set; }
    }
}