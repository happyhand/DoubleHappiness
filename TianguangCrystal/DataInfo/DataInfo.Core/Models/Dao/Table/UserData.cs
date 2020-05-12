using SqlSugar;

namespace DataInfo.Core.Models.Dao.Table
{
    /// <summary>
    /// 使用者表單資料
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// Gets or sets Birthday
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets CreateDate
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Gender
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

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