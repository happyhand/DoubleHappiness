namespace DataInfo.Core.Models.Dto.Team.Response
{
    /// <summary>
    /// 更換車隊隊長後端回覆資料
    /// </summary>
    public class TeamUpdateViceLeaderResponse
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 回覆結果
        /// </summary>
        public int Result { get; set; }
    }
}