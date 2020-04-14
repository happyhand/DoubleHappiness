namespace DataInfo.Core.Models.Dto.Member.Request
{
    /// <summary>
    /// 會員註冊請求後端資料
    /// </summary>
    public class MemberRegisterRequestDto
    {
        /// <summary>
        /// Gets or sets 確認密碼
        /// </summary>
        public string CheckPassword { get; set; }

        /// <summary>
        /// Gets or sets 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets FB認證碼
        /// </summary>
        public string FBToken { get; set; }

        /// <summary>
        /// Gets or sets Google認證碼
        /// </summary>
        public string GoogleToken { get; set; }

        /// <summary>
        /// Gets or sets 密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets 註冊來源
        /// </summary>
        public int RegisterSource { get; set; }
    }
}