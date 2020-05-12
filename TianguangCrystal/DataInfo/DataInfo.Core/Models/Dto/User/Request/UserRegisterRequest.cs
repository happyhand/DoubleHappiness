﻿namespace DataInfo.Core.Models.Dto.User.Request
{
    /// <summary>
    /// 請求使用者註冊
    /// </summary>
    public class UserRegisterRequest
    {
        /// <summary>
        /// Gets or sets Birthday
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Gender
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Gets or sets Mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets Subscribe
        /// </summary>
        public int Subscribe { get; set; }
    }
}