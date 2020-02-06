using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Service.Models.Member.data
{
    /// <summary>
    /// 會員搜尋關鍵字資料
    /// </summary>
    public class MemberSearchKeyDto
    {
        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets FBToken
        /// </summary>
        public string FBToken { get; set; }

        /// <summary>
        /// Gets or sets GoogleToken
        /// </summary>
        public string GoogleToken { get; set; }

        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }
    }
}