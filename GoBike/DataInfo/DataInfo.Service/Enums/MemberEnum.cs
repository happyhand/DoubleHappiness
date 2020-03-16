namespace DataInfo.Service.Enums
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
    /// 圖像類別資料
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// 頭像
        /// </summary>
        Avatar = 1,

        /// <summary>
        /// 封面
        /// </summary>
        FrontCover = 2
    }

    /// <summary>
    /// 在線狀態類別資料
    /// </summary>
    public enum OnlineStatusType
    {
        /// <summary>
        /// 未上線
        /// </summary>
        Offline = 0,

        /// <summary>
        /// 在線
        /// </summary>
        Online = 1
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
}