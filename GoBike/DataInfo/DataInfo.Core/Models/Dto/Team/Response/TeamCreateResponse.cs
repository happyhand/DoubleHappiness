namespace DataInfo.Core.Models.Dto.Team.Response
{
    /// <summary>
    /// 建立車隊後端回覆資料
    /// </summary>
    public class TeamCreateResponse
    {
        /// <summary>
        /// Gets or sets 回覆結果
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }
}