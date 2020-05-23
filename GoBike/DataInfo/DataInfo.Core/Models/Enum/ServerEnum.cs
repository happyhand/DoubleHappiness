namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 後端封包類別資料
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// 測試
        /// </summary>
        test = 0,

        /// <summary>
        /// 會員
        /// </summary>
        User = 1,

        /// <summary>
        /// 騎乘
        /// </summary>
        Ride = 2,

        /// <summary>
        /// 車隊
        /// </summary>
        Team = 3
    }

    #region 會員

    /// <summary>
    /// 更新互動結果類別資料
    /// </summary>
    public enum UpdateInteractiveResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1,
    }

    /// <summary>
    /// 更新使用者資訊密碼結果類別資料
    /// </summary>
    public enum UpdatePasswordResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 舊密碼錯誤
        /// </summary>
        OldPasswordError = 2,
    }

    /// <summary>
    /// 更新使用者資訊結果類別資料
    /// </summary>
    public enum UpdateUserInfoResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1,
    }

    /// <summary>
    /// 會員後端封包編號類別資料
    /// </summary>
    public enum UserCommandIDType
    {
        /// <summary>
        /// 使用者註冊
        /// </summary>
        UserRegistered = 1001,

        /// <summary>
        /// 使用者登入
        /// </summary>
        UserLogin = 1002,

        /// <summary>
        /// 更新使用者資訊
        /// </summary>
        UpdateUserInfo = 1003,

        /// <summary>
        /// 更新密碼
        /// </summary>
        UpdatePassword = 1004,

        /// <summary>
        /// 更新朋友列表
        /// </summary>
        UpdateFriendList = 1006,

        /// <summary>
        /// 更新黑名單列表
        /// </summary>
        UpdateBlackList = 1007
    }

    /// <summary>
    /// 使用者登入結果類別資料
    /// </summary>
    public enum UserLoginResultType
    {
        /// <summary>
        /// 登入失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 登入成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 帳號錯誤
        /// </summary>
        EmailError = 2,

        /// <summary>
        /// 密碼錯誤
        /// </summary>
        PasswordError = 3
    }

    /// <summary>
    /// 使用者註冊結果類別資料
    /// </summary>
    public enum UserRegisteredResultType
    {
        /// <summary>
        /// 註冊失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 註冊成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 帳號重複
        /// </summary>
        Repeat = 2,

        /// <summary>
        /// 密碼錯誤
        /// </summary>
        PasswordError = 3,

        /// <summary>
        /// 帳號格式不符
        /// </summary>
        EmailError = 4,
    }

    #endregion 會員

    #region 騎乘

    /// <summary>
    /// 建立騎乘紀錄結果類別資料
    /// </summary>
    public enum CreateRideRecordResultType
    {
        /// <summary>
        /// 建立失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 建立成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 騎乘後端封包編號類別資料
    /// </summary>
    public enum RideCommandIDType
    {
        /// <summary>
        /// 建立騎乘紀錄
        /// </summary>
        CreateRideRecord = 1001,
    }

    #endregion 騎乘

    #region 車隊

    /// <summary>
    /// 建立新車隊結果類別資料
    /// </summary>
    public enum CreateNewTeamResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1,
    }

    /// <summary>
    /// 車隊後端封包編號類別資料
    /// </summary>
    public enum TeamCommandIDType
    {
        /// <summary>
        /// 建立新車隊
        /// </summary>
        CreateNewTeam = 1001,

        /// <summary>
        /// 更新隊員列表
        /// </summary>
        UpdateTeamMemberList = 1005,

        /// <summary>
        /// 更新申請加入車隊列表
        /// </summary>
        UpdateApplyJoinList = 1006,

        /// <summary>
        /// 更新邀請加入車隊列表
        /// </summary>
        UpdateInviteJoinList = 1007,

        /// <summary>
        /// 更新活動
        /// </summary>
        UpdateActivity = 1009
    }

    /// <summary>
    /// 更新活動結果類別資料
    /// </summary>
    public enum UpdateActivityResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 權限不足
        /// </summary>
        AuthorityNotEnough = 2,
    }

    /// <summary>
    /// 更新申請加入車隊列表結果類別資料
    /// </summary>
    public enum UpdateApplyJoinListResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1,
    }

    /// <summary>
    /// 更新邀請加入車隊列表結果類別資料
    /// </summary>
    public enum UpdateInviteJoinListResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1,
    }

    /// <summary>
    /// 更新隊員列表結果類別資料
    /// </summary>
    public enum UpdateTeamMemberListResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1,
    }

    #endregion 車隊
}