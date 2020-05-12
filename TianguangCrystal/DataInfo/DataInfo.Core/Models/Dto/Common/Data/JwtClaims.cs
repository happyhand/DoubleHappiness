namespace DataInfo.Core.Models.Dto.Common.Data
{
    /// <summary>
    /// Jwy Claims 資料
    /// </summary>
    public class JwtClaims
    {
        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets UserID
        /// </summary>
        public long UserID { get; set; }
    }
}