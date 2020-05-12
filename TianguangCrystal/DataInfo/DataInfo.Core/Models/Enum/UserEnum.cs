namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 性別類別資料
    /// </summary>
    public enum GenderType
    {
        /// <summary>
        /// 未設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 女生
        /// </summary>
        Woman = 1,

        /// <summary>
        /// 男生
        /// </summary>
        Man = 2
    }

    /// <summary>
    /// 訂閱電郵類別資料
    /// </summary>
    public enum SubscribeType
    {
        /// <summary>
        /// 未申請
        /// </summary>
        None = 0,

        /// <summary>
        /// 申請
        /// </summary>
        Apply = 1,
    }

    /// <summary>
    /// 使用者註冊結果類別資料
    /// </summary>
    public enum UserRegisterResultType
    {
        /// <summary>
        /// Mobile 重覆
        /// </summary>
        MobileRepeat = -2,

        /// <summary>
        /// Email 重覆
        /// </summary>
        EmailRepeat = -1,

        /// <summary>
        /// 失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
    }
}