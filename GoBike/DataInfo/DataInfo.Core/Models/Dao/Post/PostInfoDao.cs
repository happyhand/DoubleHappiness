using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Post
{
    /// <summary>
    /// 貼文資訊資料
    /// </summary>
    public class PostInfoDao
    {
        /// <summary>
        /// Gets or sets 內文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets 貼文日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets 點讚數
        /// </summary>
        public string LikeList { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 照片
        /// </summary>
        public string Photo { get; set; }
    }
}