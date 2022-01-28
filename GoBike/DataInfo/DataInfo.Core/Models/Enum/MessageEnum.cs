namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 回覆錯誤訊息類別資料
    /// </summary>
    public enum ResponseErrorMessageType
    {
        /// <summary>
        /// 系統忙碌中，請稍後再試
        /// </summary>
        SystemError,

        /// <summary>
        /// 無會員資料，請重新操作
        /// </summary>
        MemberIDEmpty,

        /// <summary>
        /// 請輸入 Token
        /// </summary>
        TokenEmpty,

        /// <summary>
        /// 請輸入電子郵件
        /// </summary>
        EmailEmpty,

        /// <summary>
        /// 電子郵件格式不正確，請重新輸入
        /// </summary>
        EmailFormatError,

        /// <summary>
        /// 此電子郵件已註冊，請重新輸入
        /// </summary>
        EmailRepeat,

        /// <summary>
        /// 電子郵件與密碼不符合，請重新輸入
        /// </summary>
        AccountOrPasswordNotMatch,

        /// <summary>
        /// 電子郵件不存在，請重新輸入
        /// </summary>
        EmailNotExist,

        /// <summary>
        /// 請輸入密碼
        /// </summary>
        PasswordEmpty,

        /// <summary>
        /// 密碼格式不符，請輸入6-12碼英數混和
        /// </summary>
        PasswordFormatError,

        /// <summary>
        /// 請輸入新密碼
        /// </summary>
        NewPasswordEmpty,

        /// <summary>
        /// 新密碼格式不符，請輸入6-12碼英數混和
        /// </summary>
        NewPasswordFormatError,

        /// <summary>
        /// 請輸入確認密碼
        /// </summary>
        ConfirmPasswordEmpty,

        /// <summary>
        /// 確認密碼不符合，請重新輸入
        /// </summary>
        ConfirmPasswordNotMatch,

        /// <summary>
        /// 密碼錯誤，請重新輸入
        /// </summary>
        OldPasswordError,

        /// <summary>
        /// 請輸入手機號碼
        /// </summary>
        MobileEmpty,

        /// <summary>
        /// 手機號碼格式不正確，請重新輸入
        /// </summary>
        MobileFormatError,

        /// <summary>
        /// 手機號碼已被綁定，請勿重複綁定
        /// </summary>
        MobileRepeat,

        /// <summary>
        /// 請輸入驗證碼
        /// </summary>
        VerifyCodeEmpty,

        /// <summary>
        /// 驗證碼格式不符，請輸入8碼英數混和
        /// </summary>
        VerifyCodeFormatError,

        /// <summary>
        /// 系統拒絕註冊，請稍後再試
        /// </summary>
        RegisterFail,

        /// <summary>
        /// 系統拒絕登入，請稍後再試
        /// </summary>
        LoginFail,

        /// <summary>
        /// 登入來源錯誤，請稍後再試
        /// </summary>
        LoginSourceFail,

        /// <summary>
        /// 資料建立失敗，請稍後再試
        /// </summary>
        CreateFail,

        /// <summary>
        /// 資料重複，請重新操作
        /// </summary>
        RepeatFail,

        /// <summary>
        /// 資料更新失敗，請稍後再試
        /// </summary>
        UpdateFail,

        /// <summary>
        /// 無法取得資料，請稍後再試
        /// </summary>
        GetFail,

        /// <summary>
        /// 驗證碼驗證失敗，請重新輸入
        /// </summary>
        VerifyCodeFail,

        /// <summary>
        /// 上傳圖像失敗，請稍後再試
        /// </summary>
        UploadPhotoFail,

        /// <summary>
        /// 上傳頭像失敗，請稍後再試
        /// </summary>
        UploadAvatarFail,

        /// <summary>
        /// 上傳封面失敗，請稍後再試
        /// </summary>
        UploadFrontCoverFail,

        /// <summary>
        /// 上傳首頁圖像失敗，請稍後再試
        /// </summary>
        UploadHomePhotoFail,

        /// <summary>
        /// 生日日期設定錯誤，請重新輸入
        /// </summary>
        BirthdayFormatError,

        /// <summary>
        /// 請輸入搜尋關鍵字
        /// </summary>
        SearchKeyEmpty,

        /// <summary>
        /// 車隊權限不足
        /// </summary>
        TeamAuthorityNotEnough,

        /// <summary>
        /// 無法對會員本身操作，請稍後再試
        /// </summary>
        TargetSelfError,

        /// <summary>
        /// 未輸入騎乘時間，請重新輸入
        /// </summary>
        RideTimeEmpty,

        /// <summary>
        /// 未輸入騎乘距離，請重新輸入
        /// </summary>
        RideDistanceEmpty,

        /// <summary>
        /// 未輸入海拔高度，請重新輸入
        /// </summary>
        RideAltitudeEmpty,

        /// <summary>
        /// 未輸入騎乘市區，請重新輸入
        /// </summary>
        RideCountyEmpty,

        /// <summary>
        /// 未輸入騎乘路徑資料，請重新輸入
        /// </summary>
        RideRouteEmpty,

        /// <summary>
        /// 未設定騎乘封面，請重新上傳
        /// </summary>
        RidePhotoEmpty,

        /// <summary>
        /// 無車隊資料，請重新操作
        /// </summary>
        TeamIDEmpty,

        /// <summary>
        /// 無貼文資料，請重新操作
        /// </summary>
        PostIDEmpty,

        /// <summary>
        /// 無車隊活動資料，請重新操作
        /// </summary>
        ActIDEmpty,

        /// <summary>
        /// 無車隊公告資料，請重新操作
        /// </summary>
        BulletinIDEmpty,

        /// <summary>
        /// 未輸入車隊名稱，請重新輸入
        /// </summary>
        TeamNameEmpty,

        /// <summary>
        /// 未輸入所在地，請重新輸入
        /// </summary>
        CountyEmpty,

        /// <summary>
        /// 未設定加入車隊是否需先審核，請重新輸入
        /// </summary>
        ExamineStatusEmpty,

        /// <summary>
        /// 未設定是否開放搜尋，請重新輸入
        /// </summary>
        SearchStatusEmpty,

        /// <summary>
        /// 回覆失敗，請重新操作
        /// </summary>
        ReplyFail,

        /// <summary>
        /// 申請失敗，請重新操作
        /// </summary>
        ApplyFail,

        /// <summary>
        /// 通知失敗，請重新操作
        /// </summary>
        NotifyFail,

        /// <summary>
        /// 無座標資料，請重新操作
        /// </summary>
        CoordinateEmpty,

        /// <summary>
        /// 超出最大人數，請重新操作
        /// </summary>
        ExceedMaxPeople,

        /// <summary>
        /// 暱稱超出最大字數，請重新輸入
        /// </summary>
        ExceedMaxNickname,

        /// <summary>
        /// 車隊名稱超出最大字數，請重新輸入
        /// </summary>
        ExceedMaxTeamName,

        /// <summary>
        /// 車隊簡介超出最大字數，請重新輸入
        /// </summary>
        ExceedMaxTeamInfo,

        /// <summary>
        /// 車隊解散失敗，請重新操作
        /// </summary>
        DisbandFail,

        /// <summary>
        /// 踢離車隊隊員失敗，請重新操作
        /// </summary>
        KickFail,

        /// <summary>
        /// 移交隊長失敗，請重新操作
        /// </summary>
        ChangeLeaderFail,

        /// <summary>
        /// 未輸入車隊活動日期，請重新輸入
        /// </summary>
        TeamActivityActDateEmpty,

        /// <summary>
        /// 車隊活動日期設定錯誤，請重新輸入
        /// </summary>
        TeamActivityActDateFail,

        /// <summary>
        /// 未輸入車隊活動集合時間，請重新輸入
        /// </summary>
        TeamActivityMeetTimeEmpty,

        /// <summary>
        /// 車隊活動集合時間設定錯誤，請重新輸入
        /// </summary>
        TeamActivityMeetTimeFail,

        /// <summary>
        /// 未輸入車隊活動最高海拔，請重新輸入
        /// </summary>
        TeamActivityMaxAltitudeEmpty,

        /// <summary>
        /// 車隊活動最高海拔設定錯誤，請重新輸入
        /// </summary>
        TeamActivityMaxAltitudeFail,

        /// <summary>
        /// 未輸入車隊活動總距離，請重新輸入
        /// </summary>
        TeamActivityTotalDistanceEmpty,

        /// <summary>
        /// 車隊活動總距離設定錯誤，請重新輸入
        /// </summary>
        TeamActivityTotalDistanceFail,

        /// <summary>
        /// 未輸入車隊活動標題，請重新輸入
        /// </summary>
        TeamActivityTitleEmpty,

        /// <summary>
        /// 未輸入車隊公告內容，請重新輸入
        /// </summary>
        BulletinContentEmpty,

        /// <summary>
        /// 未設定車隊公告天數，請重新輸入
        /// </summary>
        BulletinDayEmpty,
    }

    /// <summary>
    /// 回覆成功訊息類別資料
    /// </summary>
    public enum ResponseSuccessMessageType
    {
        /// <summary>
        /// 已發送驗證碼至您的信箱，請於 5 分鐘內檢視並輸入驗證碼
        /// </summary>
        SendVerifierCodeSuccess,

        /// <summary>
        /// 資料建立成功
        /// </summary>
        CreateSuccess,

        /// <summary>
        /// 資料更新成功
        /// </summary>
        UpdateSuccess,

        /// <summary>
        /// 密碼更新成功，請重新登入
        /// </summary>
        UpdatePasswordSuccess,

        /// <summary>
        /// 回覆成功
        /// </summary>
        ReplySuccess,

        /// <summary>
        /// 申請成功
        /// </summary>
        ApplySuccess,

        /// <summary>
        /// 通知成功
        /// </summary>
        NotifySuccess,

        /// <summary>
        /// 車隊解散成功
        /// </summary>
        DisbandSuccess,

        /// <summary>
        /// 踢離車隊隊員成功
        /// </summary>
        KickSuccess,

        /// <summary>
        /// 移交隊長成功
        /// </summary>
        ChangeLeaderSuccess,
    }
}