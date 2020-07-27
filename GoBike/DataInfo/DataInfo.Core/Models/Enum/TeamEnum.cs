namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 可活動動作狀態類別資料
    /// </summary>
    public enum ActivityActionStatusType
    {
        /// <summary>
        /// 取消活動
        /// </summary>
        Delete = -1,

        /// <summary>
        /// 取消加入
        /// </summary>
        Cancel = 0,

        /// <summary>
        /// 加入活動
        /// </summary>
        Join = 1
    }

    /// <summary>
    /// 加入狀態類別資料
    /// </summary>
    public enum JoinStatusType
    {
        /// <summary>
        /// 未加入
        /// </summary>
        None = 0,

        /// <summary>
        /// 在線
        /// </summary>
        Join = 1
    }

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
    /// 車隊互動類別資料
    /// </summary>
    public enum TeamInteractiveType
    {
        /// <summary>
        /// 無設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 申請加入
        /// </summary>
        ApplyJoin = 1,

        /// <summary>
        /// 成員
        /// </summary>
        Member = 2
    }

    /// <summary>
    /// 車隊回覆類別資料
    /// </summary>
    public enum TeamResponseType
    {
        /// <summary>
        /// 拒絕
        /// </summary>
        Reject = -1,

        /// <summary>
        /// 無設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 允許
        /// </summary>
        Allow = 1
    }

    /// <summary>
    /// 車隊角色類別資料
    /// </summary>
    public enum TeamRoleType
    {
        /// <summary>
        /// 非隊員
        /// </summary>
        None = 0,

        /// <summary>
        /// 隊員
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 副隊長
        /// </summary>
        ViceLeader = 2,

        /// <summary>
        /// 隊長
        /// </summary>
        Leader = 3
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
}