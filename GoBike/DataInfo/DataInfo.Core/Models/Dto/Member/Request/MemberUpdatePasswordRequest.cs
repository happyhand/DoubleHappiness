namespace DataInfo.Core.Models.Dto.Member.Request
{
    /// <summary>
    /// 會員更新密碼請求後端資料
    /// </summary>
    public class MemberUpdatePasswordRequest
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets NewPassword
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }
    }
}