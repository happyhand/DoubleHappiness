namespace DataInfo.Core.Models.Dto.Member.Request
{
    /// <summary>
    /// 會員登入請求後端資料
    /// </summary>
    public class MemberLoginRequest
    {
        /// <summary>
        /// Gets or sets 註冊來源
        /// </summary>
        public string LoginSource { get; set; }

        /// <summary>
        /// Gets or sets Token
        /// </summary>
        public string Token { get; set; }
    }
}