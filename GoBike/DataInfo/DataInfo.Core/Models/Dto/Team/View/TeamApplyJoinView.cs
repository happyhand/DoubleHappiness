namespace DataInfo.Core.Models.Dto.Team.View
{
    /// <summary>
    /// 申請加入車隊可視資料
    /// </summary>
    public class TeamApplyJoinView
    {
        /// <summary>
        /// Gets or sets 審核狀態(1:開，-1:關)
        /// </summary>
        public int ExamineStatus { get; set; }
    }
}