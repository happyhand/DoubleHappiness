namespace DataInfo.Core.Models.Dao.Member.Table
{
    /// <summary>
    /// 使用者帳戶
    /// </summary>
    public class UserAccount
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

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets RegisterDate
        /// </summary>
        public string RegisterDate { get; set; }

        /// <summary>
        /// Gets or sets RegisterSource
        /// </summary>
        public int RegisterSource { get; set; }
    }
}