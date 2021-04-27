using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Ride.Request
{
    /// <summary>
    /// 新增貼文請求後端資料
    /// </summary>
    public class AddPostRequest
    {
        /// <summary>
        /// Gets or sets 內文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 圖片
        /// </summary>
        public string Photo { get; set; }
    }
}