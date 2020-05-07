namespace DataInfo.Core.Models.Dto.Ride.View
{
    /// <summary>
    /// 好友週里程排名可視資料
    /// </summary>
    public class RideFriendWeekRankView
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets 週里程
        /// </summary>
        public float WeekDistance { get; set; }
    }
}