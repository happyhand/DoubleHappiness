namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 動作類別資料
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// 刪除
        /// </summary>
        Delete = -1,

        /// <summary>
        /// 無動作
        /// </summary>
        None = 0,

        /// <summary>
        /// 新增
        /// </summary>
        Add = 1,

        /// <summary>
        /// 修改
        /// </summary>
        Edit = 2
    }

    /// <summary>
    /// 騎乘等級類別資料
    /// </summary>
    public enum RideLevelType
    {
        /// <summary>
        /// 無設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 無設定
        /// </summary>
        Easy = 1,

        /// <summary>
        /// 無設定
        /// </summary>
        Normal = 2,

        /// <summary>
        /// 無設定
        /// </summary>
        Hard = 3
    }

    /// <summary>
    /// 驗證類別資料
    /// </summary>
    public enum VerifierType
    {
        /// <summary>
        /// 忘記密碼
        /// </summary>
        ForgetPassword = 1,

        /// <summary>
        /// 綁定行動電話
        /// </summary>
        MobileBind = 2,
    }
}