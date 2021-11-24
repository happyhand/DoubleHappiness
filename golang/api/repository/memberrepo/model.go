package memberrepo

// MemberDao : 會員資料
type MemberDao struct {
	Avatar         string  `db:"Avatar" joindb:"ui.Avatar"`                 // 頭像路徑
	Birthday       string  `db:"Birthday" joindb:"ui.Birthday"`             // 生日
	BlackList      string  `db:"BlackList" joindb:"ui.BlackList"`           // 黑名單列表
	BodyHeight     float64 `db:"BodyHeight" joindb:"ui.BodyHeight"`         // 身高
	BodyWeight     float64 `db:"BodyWeight" joindb:"ui.BodyWeight"`         // 體重
	County         int     `db:"County" joindb:"ui.County"`                 // 居住地
	Email          string  `db:"Email" joindb:"ui.Email"`                   // 信箱
	FriendList     string  `db:"FriendList" joindb:"ui.FriendList"`         // 好友列表
	FrontCover     string  `db:"FrontCover" joindb:"ui.FrontCover"`         // 封面圖片路徑
	Gender         int     `db:"Gender" joindb:"ui.Gender"`                 // 性別
	MemberID       string  `db:"MemberID" joindb:"ua.MemberID"`             // 會員 ID
	Mobile         string  `db:"Mobile" joindb:"ui.Mobile"`                 // 手機
	Nickname       string  `db:"NickName" joindb:"ui.NickName"`             // 暱稱
	Photo          string  `db:"Photo" joindb:"ui.Photo"`                   // 首頁圖片路徑
	RegisterDate   string  `db:"RegisterDate" joindb:"ua.RegisterDate"`     // 註冊日期
	RegisterSource int     `db:"RegisterSource" joindb:"ua.RegisterSource"` // 註冊來源
	TeamList       string  `db:"TeamList" joindb:"ui.TeamList"`             // 車隊列表

	// TODO 待加入 db
	SearchStatus int // 是否開放搜尋 0:不開放，1:開放
}
