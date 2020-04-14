using SqlSugar;

namespace DataInfo.Core.Models.Dao.Member
{
    /// <summary>
    /// 互動資料
    /// </summary>
    public class InteractiveModel
    {
        /// <summary>
        /// Gets or sets 建立者會員ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string CreatorID { get; set; }

        /// <summary>
        /// Gets or sets 狀態
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets 目標會員ID
        /// </summary>
        public string TargetID { get; set; }
    }
}