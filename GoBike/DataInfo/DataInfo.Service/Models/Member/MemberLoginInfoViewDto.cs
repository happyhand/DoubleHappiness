using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Service.Models.Member
{
    /// <summary>
    /// 會員登入可視資料
    /// </summary>
    public class MemberLoginInfoViewDto
    {
        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public long MemberID { get; set; }

        /// <summary>
        /// Gets or sets ServerIP
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// Gets or sets ServerPort
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets Token
        /// </summary>
        public string Token { get; set; }
    }
}