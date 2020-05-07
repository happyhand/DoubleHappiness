namespace DataInfo.Core.Models.Dao.Interactive
{
    /// <summary>
    /// 互動資料
    /// </summary>
    public class InteractiveDao
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 狀態
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets 目標會員 ID
        /// </summary>
        public string TargetID { get; set; }
    }
}