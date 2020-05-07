namespace DataInfo.Core.Models.Dto.Interactive.Response
{
    /// <summary>
    /// 更新互動狀態後端回覆資料
    /// </summary>
    public class UpdateInteractiveResponse
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