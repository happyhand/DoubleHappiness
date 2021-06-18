namespace DataInfo.Core.Models.Dto.Member.Request
{
    /// <summary>
    /// 會員登入請求後端資料
    /// </summary>
    public class MemberLoginRequest
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets 登入來源
        /// </summary>
        public int LoginSource { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets FireBase認證用的Token
        /// </summary>
        public string Token { get; set; }
    }
}