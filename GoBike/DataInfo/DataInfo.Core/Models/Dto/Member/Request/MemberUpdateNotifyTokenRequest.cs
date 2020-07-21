namespace DataInfo.Core.Models.Dto.Member.Request
{
    /// <summary>
    /// 會員更新推播 Token 請求後端資料
    /// </summary>
    public class MemberUpdateNotifyTokenRequest
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets NotifyToken
        /// </summary>
        public string NotifyToken { get; set; }
    }
}