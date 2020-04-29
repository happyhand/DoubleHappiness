namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 車隊審核狀態類別資料
    /// </summary>
    public enum TeamExamineStatusType
    {
        /// <summary>
        /// 無設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 關閉
        /// </summary>
        Close = -1,

        /// <summary>
        /// 開啟
        /// </summary>
        Open = 1
    }

    /// <summary>
    /// 車隊搜尋狀態類別資料
    /// </summary>
    public enum TeamSearchStatusType
    {
        /// <summary>
        /// 無設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 關閉
        /// </summary>
        Close = -1,

        /// <summary>
        /// 開啟
        /// </summary>
        Open = 1
    }

    /// <summary>
    /// 車隊搜尋類別資料
    /// </summary>
    public enum TeamSearchType
    {
        /// <summary>
        /// 車隊 ID
        /// </summary>
        TeamID = 1,

        /// <summary>
        /// 車隊名稱
        /// </summary>
        TeamName = 2,
    }
}