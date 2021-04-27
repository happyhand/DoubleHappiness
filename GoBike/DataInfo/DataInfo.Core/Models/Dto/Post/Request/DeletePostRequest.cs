using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Ride.Request
{
    /// <summary>
    /// 刪除貼文請求後端資料
    /// </summary>
    public class DeletePostRequest
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets PostID ID
        /// </summary>
        public string PostID { get; set; }
    }
}