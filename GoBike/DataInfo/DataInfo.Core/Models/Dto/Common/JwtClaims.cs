using System;

namespace DataInfo.Core.Models.Dto.Common
{
    /// <summary>
    /// Jwy Claims 資料
    /// </summary>
    public class JwtClaims
    {
        /// <summary>
        /// Gets or sets Avatar
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets FrontCover
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets Mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets Nickname
        /// </summary>
        public string Nickname { get; set; }
    }
}