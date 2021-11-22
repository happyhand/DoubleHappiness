package config

import "time"

// #region DB
const (
	// CommonDB : common db
	CommonDB int = 6
	// MemberDB : member db
	MemberDB int = 7
	// RideDB : ride db
	RideDB int = 8
)

// #endregion

// #region Common Key
const (
	// VerificationCodeRedisKey : 驗證碼 redis key (類型, 發送信箱)
	VerificationCodeRedisKey string = "VerificationCode-%d-%s"

	// LockRegisterRedisKey : lock register redis key
	LockRegisterRedisKey string = "LockRegister_%s"
	// LockMobileRedisKey : lock mobile redis key
	LockMobileRedisKey string = "LockMobile_%s"
)

// #endregion

// #region Member Key
// U(update):當會員資訊更新後，該 key 需刪除，以便更新 redis 資料
const (
	// MemberLastLoginRedisKey : 會員最新登入 redis key
	MemberLastLoginRedisKey string = "MemberLastLogin-%s"
	// MemberCardInfoRedisKey : 會員名片資訊 redis key (查詢者ID, 查詢對象ID)
	MemberCardInfoRedisKey string = "MemberCardInfo-%s-U-%s"
	// MemberHomeInfoRedisKey : 會員首頁資訊 redis key
	MemberHomeInfoRedisKey string = "MemberCardInfo-U-%s"
	// MemberInfoRedisKey : 會員資訊 redis key
	MemberInfoRedisKey string = "MemberInfo-U-%s"
	// MemberSearchRedisKey : 會員搜尋 redis key (關鍵字, 是否模糊搜尋, 查詢者ID)
	MemberSearchRedisKey string = "MemberSearch-%s-%d-%s"
)

// #endregion

// #region Ride Key
// U(update):當會員資訊更新後，該 key 需刪除，以便更新 redis 資料
const (
	// RideRecordRedisKey : 騎乘紀錄資訊 redis key
	RideRecordRedisKey string = "RideRecord-U-%s"
	// RideRecordDetailRedisKey : 騎乘紀錄明細資訊 redis key
	RideRecordDetailRedisKey string = "RideRecordDetail-%s"
)

// #endregion

// #region Time Out
const (
	// VerificationCodeTimeout : 驗證碼過期時間(10m)
	VerificationCodeTimeout time.Duration = 10 * time.Minute
	// KeepOnlineTimeout : 會員在線過期時間(5m)
	KeepOnlineTimeout time.Duration = 5 * time.Minute

	// MemberCardInfoTimeout : 會員名片資訊過期時間(1m)
	MemberCardInfoTimeout time.Duration = 1 * time.Minute
	// MemberHomeInfoTimeout : 會員首頁資訊過期時間(1m)
	MemberHomeInfoTimeout time.Duration = 1 * time.Minute
	// MemberInfoTimeout : 會員資訊過期時間(1m)
	MemberInfoTimeout time.Duration = 1 * time.Minute
	// MemberSearchTimeout : 會員搜尋過期時間(30s)
	MemberSearchTimeout time.Duration = 30 * time.Second

	// RideInfoTimeout : 騎乘紀錄過期時間(1m)
	RideInfoTimeout time.Duration = 1 * time.Minute
)

// #endregion
