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
        /// 邀請加入
        /// </summary>
        InviteJoin = 2,

        /// <summary>
        /// 成員
        /// </summary>
        Member = 3
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
    /// 車隊角色類型資料
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