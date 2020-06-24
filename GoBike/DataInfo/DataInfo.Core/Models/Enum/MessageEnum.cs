namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 回覆錯誤訊息類別資料
    /// </summary>
    public enum ResponseErrorMessageType
    {
        /// <summary>
        /// 系統忙碌中，請稍後再試
        /// </summary>
        SystemError,

        /// <summary>
        /// 請輸入電子郵件
        /// </summary>
        EmailEmpty,

        /// <summary>
        /// 電子郵件格式不正確，請重新輸入
        /// </summary>
        EmailFormatError,

        /// <summary>
        /// 此電子郵件已註冊，請重新輸入
        /// </summary>
        EmailRepeat,

        /// <summary>
        /// 請輸入密碼
        /// </summary>
        PasswordEmpty,

        /// <summary>
        /// 密碼格式不符，請輸入6-12碼英數混和
        /// </summary>
        PasswordFormatError,

        /// <summary>
        /// 請輸入確認密碼
        /// </summary>
        ConfirmPasswordEmpty,

        /// <summary>
        /// 確認密碼不符合，請重新輸入
        /// </summary>
        ConfirmPasswordNotMatch,

        /// <summary>
        /// 系統拒絕註冊，請稍後再試
        /// </summary>
        RegisterFail,

        /// <summary>
        /// 電子郵件與密碼不符合，請重新輸入
        /// </summary>
        EmailOrPasswordNotMatch,

        /// <summary>
        /// 系統拒絕登入，請稍後再試
        /// </summary>
        LoginFail,
    }
}