using DataInfo.Core.Applibs;

namespace DataInfo.Core.Models.Dto.Common
{
    /// <summary>
    /// 郵件資料
    /// </summary>
    public class EmailContext
    {
        /// <summary>
        /// Gets or sets Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets Body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 取得重設密碼郵件內容
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <returns>EmailContext</returns>
        public static EmailContext GetResetPasswordEmailContext(string email, string password)
        {
            return new EmailContext()
            {
                Address = email,
                Body = $"<p>親愛的用戶您好</p>" +
                       $"<p>已將您的密碼重新設定為</p>" +
                       $"<p><span style='font-weight:bold; color:blue;'>{password}</span></p>" +
                       $"<p>請於重新登入後，於設定個人檔案重新設定密碼</p>" +
                       $"<br><br><br>" +
                       $"<p>※本電子郵件係由系統自動發送，請勿直接回覆本郵件。</p>",
                Subject = "GoBike 查詢密碼"
            };
        }

        /// <summary>
        /// 取得忘記密碼驗證碼郵件內容
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="verifierCode">verifierCode</param>
        /// <returns>EmailContext</returns>
        public static EmailContext GetVerifierCodetEmailContextForForgetPassword(string email, string verifierCode)
        {
            return new EmailContext()
            {
                Address = email,
                Body = $"<p>親愛的用戶您好</p>" +
                       $"<p>您的驗證碼為</p>" +
                       $"<p><span style='font-weight:bold; color:blue;'>{verifierCode}</span></p>" +
                       $"<p>請於 <span style='font-weight:bold; color:blue;'>{AppSettingHelper.Appsetting.VerifierCodeExpirationDate}分鐘</span> 內於APP輸入此驗證碼以獲取新密碼</p>" +
                       $"<br><br><br>" +
                       $"<p>※本電子郵件係由系統自動發送，請勿直接回覆本郵件。</p>",
                Subject = "GoBike 查詢密碼"
            };
        }

        /// <summary>
        /// 取得綁定行動電話驗證碼郵件內容
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="verifierCode">verifierCode</param>
        /// <returns>EmailContext</returns>
        public static EmailContext GetVerifierCodetEmailContextForMobileBind(string email, string verifierCode)
        {
            return new EmailContext()
            {
                Address = email,
                Body = $"<p>親愛的用戶您好</p>" +
                       $"<p>您的驗證碼為</p>" +
                       $"<p><span style='font-weight:bold; color:blue;'>{verifierCode}</span></p>" +
                       $"<p>請於 <span style='font-weight:bold; color:blue;'>{AppSettingHelper.Appsetting.VerifierCodeExpirationDate}分鐘</span> 內於APP輸入此驗證碼以綁定行動電話</p>" +
                       $"<br><br><br>" +
                       $"<p>※本電子郵件係由系統自動發送，請勿直接回覆本郵件。</p>",
                Subject = "GoBike 行動電話綁定作業"
            };
        }
    }
}