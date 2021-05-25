using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Post.View
{
    /// <summary>
    /// 貼文資訊可視資料
    /// </summary>
    public class PostInfoView
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 內文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets 貼文日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets 是否已點讚
        /// </summary>
        public int IsLike { get; set; }

        /// <summary>
        /// Gets or sets 點讚數
        /// </summary>
        public int Like { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets 照片
        /// </summary>
        public IEnumerable<string> Photo { get; set; }

        /// <summary>
        /// Gets or sets 貼文 ID
        /// </summary>
        public string PostID { get; set; }
    }
}