package memberservice

import "time"

const (
	// SearchEmail : 搜尋類型 - email
	SearchEmail int = 1
	// SearchMemberID : 搜尋類型 - member id
	SearchMemberID int = 2
	// SearchNickname : 搜尋類型 - nickname
	SearchNickname int = 3
)

const (
	// RegisterSourceNormal : 會員一般註冊
	RegisterSourceNormal int = 0
	// RegisterSourceFB : 會員 FB 註冊
	RegisterSourceFB int = 1
	// RegisterSourceGoogle : 會員 Google 註冊
	RegisterSourceGoogle int = 2
)

// LoginParams : 會員登入參數內容
type LoginParams struct {
	Avatar      string
	Email       string
	LoginSource int
	NickName    string
	Token       string
}

// ResetPasswordParams : 會員重置密碼參數內容
type ResetPasswordParams struct {
	Email    string
	Password string
}

// UpdagtePasswordParams : 會員更新密碼參數內容
type UpdagtePasswordParams struct {
	MemberID    string
	NewPassword string
	Password    string
}

// SearchParams : 搜尋會員參數內容
type SearchParams struct {
	Key               string
	MemberID          string
	IsFuzzy           bool
	CheckSearchStatus bool // TODO 待確認會員卡片資訊是否提供給他人查看
}

// GetInfoParams : 取得會員資訊參數內容
type GetInfoParams struct {
	SearchID string
	TargetID string
}

// EditInfoParams : 會員編輯資訊參數內容
type EditInfoParams struct {
	MemberID   string
	Avatar     *string
	Birthday   *time.Time
	BodyHeight *float64
	BodyWeight *float64
	County     *int
	FrontCover *string
	Gender     *int
	Nickname   *string
	Photo      *string
}

// UpdateNotifyTokenParams : 會員更新 notify token 參數內容
type UpdateNotifyTokenParams struct {
	MemberID    string
	NotifyToken string
}

// MobileBindParams : 會員手機綁定參數內容
type MobileBindParams struct {
	MemberID string
	Mobile   string
}

// MemberDto : 會員資訊
type MemberDto struct {
	Avatar     string
	Birthday   string
	BodyHeight float64
	BodyWeight float64
	County     int
	Email      string
	FrontCover string
	Gender     int
	HasTeam    int
	MemberID   string
	Mobile     string
	Nickname   string
	OnlineType int
	Photo      string
}
