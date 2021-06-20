using SqlSugar;

namespace DataInfo.Core.Models.Dao.Member.Table
{
    /// <summary>
    /// 使用者帳戶
    /// </summary>
    public class UserAccount
    {
        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets NotifyToken
        /// </summary>
        public string NotifyToken { get; set; }

        /// <summary>
        /// Gets or sets RegisterDate
        /// </summary>
        public string RegisterDate { get; set; }

        /// <summary>
        /// Gets or sets RegisterSource
        /// </summary>
        public int RegisterSource { get; set; }

        /// <summary>
        /// Gets or sets UID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string UID { get; set; }
    }
}