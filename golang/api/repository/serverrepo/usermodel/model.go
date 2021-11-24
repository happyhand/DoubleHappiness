package usermodel

// #region 後端封包編號類別資料
const (
	// UserLoginCommandID : 使用者登入
	UserLoginCommandID int = 1001
	// UpdateUserInfoCommandID : 更新使用者資訊
	UpdateUserInfoCommandID int = 1002
	// UpdateFriendListCommandID : 更新朋友列表
	UpdateFriendListCommandID int = 1003
	// UpdateBlackListCommandID : 更新黑名單列表
	UpdateBlackListCommandID int = 1004
	// UpdateNotifyTokenCommandID : 更新推播 Token
	UpdateNotifyTokenCommandID int = 1005
	// UpdateNotifyTokenCommandID : 取得新增好友名單
	GetNewFriendListCommandID int = 1006
)

// #endregion

// #region 使用者登入相關

// 使用者登入結果類別資料
const (
	// UserLoginResultFail : 登入失敗
	UserLoginResultFail int = 0
	// UserLoginResultSuccess : 登入成功
	UserLoginResultSuccess int = 1
	// UserLoginResultEmailError : 帳號錯誤
	UserLoginResultEmailError int = 2
	// UserLoginResultPasswordError : 密碼錯誤
	UserLoginResultPasswordError int = 3
)

// LoginRequest : 會員登入請求
type LoginRequest struct {
	Avatar      string
	Email       string
	LoginSource int
	NickName    string
	Token       string
}

// LoginResponse : 會員登入請求結果
type LoginResponse struct {
	MemberID string
	Result   int
}

// #endregion

// #region 更新使用者資訊相關
// 更新使用者資訊結果類別資料
const (
	// UpdateUserInfoResultFail : 更新失敗
	UpdateUserInfoResultFail int = 0
	// UpdateUserInfoResultSuccess : 更新成功
	UpdateUserInfoResultSuccess int = 1
)

// EditInfoRequest : 會員編輯資訊請求
type EditInfoRequest struct {
	MemberID   string
	UpdateData struct {
		Avatar     *string
		Birthday   *string
		BodyHeight *float64
		BodyWeight *float64
		County     *int
		FrontCover *string
		Gender     *int
		Mobile     *string
		NickName   *string
		Photo      *string
	}
}

// EditInfoResponse : 會員編輯資訊請求結果
type EditInfoResponse struct {
	Result int
}

// #endregion

// #region 更新使用者密碼相關

// 會員更新密碼動作類別資料
const (
	// UpdatePasswordAction : 會員更新密碼
	UpdatePasswordAction int = 1
	// ResetPasswordAction : 會員重置密碼
	ResetPasswordAction int = 2
)

// 更新使用者密碼結果類別資料
const (
	// UpdatePasswordResultFail : 更新失敗
	UpdatePasswordResultFail int = 0
	// UpdatePasswordResultSuccess : 更新成功
	UpdatePasswordResultSuccess int = 1
	// UpdatePasswordResultOldPasswordError : 舊密碼錯誤
	UpdatePasswordResultOldPasswordError int = 2
)

// UpdatePasswordRequest : 會員更新密碼請求
type UpdatePasswordRequest struct {
	Action      int
	MemberID    string
	NewPassword string
	Password    string
}

// UpdatePasswordResponse : 會員更新密碼請求結果
type UpdatePasswordResponse struct {
	Result int
}

// #endregion

// #region 更新推播 Token 相關
// 更新推播 Token 結果類別資料
const (
	// UpdateNotifyTokenResultFail : 更新失敗
	UpdateNotifyTokenResultFail int = 0
	// UpdateNotifyTokenResultSuccess : 更新成功
	UpdateNotifyTokenResultSuccess int = 1
)

// UpdateNotifyTokenRequest : 更新推播 Token 請求
type UpdateNotifyTokenRequest struct {
	MemberID    string
	NotifyToken string
}

// UpdateNotifyTokenResponse : 更新推播 Token 請求結果
type UpdateNotifyTokenResponse struct {
	Result int
}

// #endregion
