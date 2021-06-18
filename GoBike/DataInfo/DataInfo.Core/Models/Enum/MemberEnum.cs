namespace DataInfo.Core.Models.Enum
{
    #region Content 相關

    /// <summary>
    /// 綁定手機狀態類別資料
    /// </summary>
    public enum BindMobileStatusType
    {
        /// <summary>
        /// 未綁定
        /// </summary>
        None = 0,

        /// <summary>
        /// 已綁定
        /// </summary>
        Bind = 1
    }

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
    /// 在線狀態類別資料
    /// </summary>
    public enum OnlineStatusType
    {
        /// <summary>
        /// 未上線
        /// </summary>
        Offline = -1,

        /// <summary>
        /// 未設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 在線
        /// </summary>
        Online = 1
    }

    #endregion Content 相關

    /// <summary>
    /// 會員互動類別資料
    /// </summary>
    public enum InteractiveType
    {
        /// <summary>
        /// 黑名單
        /// </summary>
        Black = -1,

        /// <summary>
        /// 好友
        /// </summary>
        Friend = 1
    }

    /// <summary>
    /// 會員登入來源類別資料
    /// </summary>
    public enum LoginSourceType
    {
        /// <summary>
        /// 一般註冊
        /// </summary>
        Normal = 0,

        /// <summary>
        /// FB
        /// </summary>
        FB = 1,

        /// <summary>
        /// Google
        /// </summary>
        Google = 2
    }

    /// <summary>
    /// 會員註冊來源類別資料
    /// </summary>
    public enum RegisterSourceType
    {
        /// <summary>
        /// 一般註冊
        /// </summary>
        Normal = 0,

        /// <summary>
        /// FB
        /// </summary>
        FB = 1,

        /// <summary>
        /// Google
        /// </summary>
        Google = 2
    }

    /// <summary>
    /// 搜尋類別資料
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// 嚴格比對
        /// </summary>
        Strict = 0,

        /// <summary>
        /// 模糊比對
        /// </summary>
        Fuzzy = 1
    }

    /// <summary>
    /// 更新密碼動作類別資料
    /// </summary>
    public enum UpdatePasswordActionType
    {
        /// <summary>
        /// 未設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 更新密碼
        /// </summary>
        Update = 1,

        /// <summary>
        /// 忘記密碼
        /// </summary>
        Forget = 2
    }
}