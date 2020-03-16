using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Service.Models.Member.Content
{
    /// <summary>
    /// 會員註冊內容
    /// </summary>
    public class MemberRegisterContent
    {
        /// <summary>
        /// Gets or sets ConfirmPassword
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }
    }
}