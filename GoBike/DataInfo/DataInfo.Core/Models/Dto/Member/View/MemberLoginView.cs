namespace DataInfo.Core.Models.Dto.Member.View
{
    /// <summary>
    /// 會員登入回應客端資料
    /// </summary>
    public class MemberLoginView
    {
        /// <summary>
        /// Gets or sets Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets RefreshToken
        /// </summary>
        public string RefreshToken { get; set; }
    }
}