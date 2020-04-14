namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 市區類別資料
    /// </summary>
    public enum CityType
    {
        /// <summary>
        /// 無設定
        /// </summary>
        None = 0
    }

    /// <summary>
    /// 騎乘等級類別資料
    /// </summary>
    public enum RideLevelType
    {
        /// <summary>
        /// 無設定
        /// </summary>
        None = 0
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