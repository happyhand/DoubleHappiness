namespace DataInfo.Core.Models.Dto.Member.Request
{
    /// <summary>
    /// 會員登入請求後端資料
    /// </summary>
    public class MemberLoginRequest
    {
        /// <summary>
        /// Gets or sets 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets 密碼
        /// </summary>
        public string Password { get; set; }
    }
}