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
        Team = 3,

        /// <summary>
        /// 貼文
        /// </summary>
        Post = 4,
    }

    #region 會員

    /// <summary>
    /// 取得新增好友名單結果類別資料
    /// </summary>
    public enum GetNewFriendListResultType
    {
        /// <summary>
        /// 取得失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 取得成功
        /// </summary>
        Success = 1,
    }

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
    /// 更新推播 Token 結果類別資料
    /// </summary>
    public enum UpdateNotifyTokenResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1
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
        /// 使用者登入
        /// </summary>
        UserLogin = 1001,

        /// <summary>
        /// 更新使用者資訊
        /// </summary>
        UpdateUserInfo = 1002,

        /// <summary>
        /// 更新朋友列表
        /// </summary>
        UpdateFriendList = 1003,

        /// <summary>
        /// 更新黑名單列表
        /// </summary>
        UpdateBlackList = 1004,

        /// <summary>
        /// 更新推播 Token
        /// </summary>
        UpdateNotifyToken = 1005,

        /// <summary>
        /// 取得新增好友名單
        /// </summary>
        GetNewFriendList = 1006
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
        AccountError = 2,

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
    /// 通知隊友結果類別資料
    /// </summary>
    public enum NotifyRideGroupMemberResultType
    {
        /// <summary>
        /// 通知失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 通知成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 回覆組隊騎乘結果類別資料
    /// </summary>
    public enum ReplyRideGroupResultType
    {
        /// <summary>
        /// 回覆失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 回覆成功
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

        /// <summary>
        /// 更新組隊騎乘
        /// </summary>
        UpdateRideGroup = 1002,

        /// <summary>
        /// 更新邀請列表
        /// </summary>
        UpdateInviteList = 1003,

        /// <summary>
        /// 回覆組隊騎乘
        /// </summary>
        ReplyRideGroup = 1004,

        /// <summary>
        /// 更新座標
        /// </summary>
        UpdateCoordinate = 1005,

        /// <summary>
        /// 通知隊友
        /// </summary>
        NotifyRideGroupMember = 1006
    }

    /// <summary>
    /// 更新座標結果類別資料
    /// </summary>
    public enum UpdateCoordinateResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 更新邀請列表結果類別資料
    /// </summary>
    public enum UpdateInviteListResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 更新組隊騎乘結果類別資料
    /// </summary>
    public enum UpdateRideGroupResultType
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
        /// 組隊已存在
        /// </summary>
        Repeat = 2
    }

    #endregion 騎乘

    #region 車隊

    /// <summary>
    /// 更換隊長結果類別資料
    /// </summary>
    public enum ChangeLeaderResultType
    {
        /// <summary>
        /// 更換失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更換成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 重複擔任
        /// </summary>
        Repeat = 2,

        /// <summary>
        /// 權限不足
        /// </summary>
        AuthorityNotEnough = 3,
    }

    /// <summary>
    /// 建立新車隊結果類別資料
    /// </summary>
    public enum CreateNewTeamResultType
    {
        /// <summary>
        /// 建立失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 建立成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 重複建立
        /// </summary>
        Repeat = 2,
    }

    /// <summary>
    /// 解散車隊結果類別資料
    /// </summary>
    public enum DeleteTeamResultType
    {
        /// <summary>
        /// 解散失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 解散成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 權限不足
        /// </summary>
        AuthorityNotEnough = 2,
    }

    /// <summary>
    /// 加入或離開車隊活動結果類別資料
    /// </summary>
    public enum JoinOrLeaveTeamActivityResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 加入或離開車隊結果類別資料
    /// </summary>
    public enum JoinOrLeaveTeamResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 踢離車隊成員結果類別資料
    /// </summary>
    public enum KickTeamMemberResultType
    {
        /// <summary>
        /// 踢離失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 踢離成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 權限不足
        /// </summary>
        AuthorityNotEnough = 2,
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
        /// 更新車隊資訊
        /// </summary>
        UpdateTeamData = 1002,

        /// <summary>
        /// 更換隊長
        /// </summary>
        ChangeLeader = 1003,

        /// <summary>
        /// 更新副隊長列表
        /// </summary>
        UpdateViceLeaderList = 1004,

        /// <summary>
        /// 更新隊員列表
        /// </summary>
        UpdateTeamMemberList = 1005,

        /// <summary>
        /// 更新申請加入車隊列表
        /// </summary>
        UpdateApplyJoinList = 1006,

        /// <summary>
        /// 更新公告
        /// </summary>
        UpdateBulletin = 1007,

        /// <summary>
        /// 更新活動
        /// </summary>
        UpdateActivity = 1008,

        /// <summary>
        /// 解散車隊
        /// </summary>
        DeleteTeam = 1009,

        /// <summary>
        /// 加入或離開車隊活動
        /// </summary>
        JoinOrLeaveTeamActivity = 1010,

        /// <summary>
        /// 加入或離開車隊
        /// </summary>
        JoinOrLeaveTeam = 1011,

        /// <summary>
        /// 踢離車隊成員
        /// </summary>
        KickTeamMember = 1012,
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
    /// 更新公告結果類別資料
    /// </summary>
    public enum UpdateBulletinResultType
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
    /// 更新車隊資訊結果類別資料
    /// </summary>
    public enum UpdateTeamDataResultType
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

    /// <summary>
    /// 更新副隊長列表結果類別資料
    /// </summary>
    public enum UpdateViceLeaderListResultType
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

    #endregion 車隊

    #region 貼文

    /// <summary>
    /// 新增點讚數結果類別資料
    /// </summary>
    public enum AddPraiseResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 建立貼文結果類別資料
    /// </summary>
    public enum CreateNewPostResultType
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
    /// 刪除貼文結果類別資料
    /// </summary>
    public enum DeletePostResultType
    {
        /// <summary>
        /// 刪除失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 刪除成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 貼文後端封包編號類別資料
    /// </summary>
    public enum PostCommandIDType
    {
        /// <summary>
        /// 建立貼文
        /// </summary>
        CreateNewPost = 1001,

        /// <summary>
        /// 更新貼文
        /// </summary>
        UpdatePost = 1002,

        /// <summary>
        /// 刪除貼文
        /// </summary>
        DeletePost = 1003,

        /// <summary>
        /// 新增點讚數
        /// </summary>
        AddLike = 1004,

        /// <summary>
        /// 減少點讚數
        /// </summary>
        ReduceLike = 1005,
    }

    /// <summary>
    /// 減少點讚數結果類別資料
    /// </summary>
    public enum ReducePraiseResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// 更新貼文結果類別資料
    /// </summary>
    public enum UpdatePostResultType
    {
        /// <summary>
        /// 更新失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 更新成功
        /// </summary>
        Success = 1
    }

    #endregion 貼文
}