package ridemodel

// #region 後端封包編號類別資料
const (
	// CreateRideRecordCommandID : 建立騎乘紀錄
	CreateRideRecordCommandID int = 1001
	// CreateRideRecordCommandID : 更新組隊騎乘
	UpdateRideGroupCommandID int = 1002
	// CreateRideRecordCommandID : 更新邀請列表
	UpdateInviteListCommandID int = 1003
	// CreateRideRecordCommandID : 回覆組隊騎乘
	ReplyRideGroupCommandID int = 1004
	// CreateRideRecordCommandID : 更新座標
	UpdateCoordinateCommandID int = 1005
	// CreateRideRecordCommandID : 通知隊友
	NotifyRideGroupMemberCommandID int = 1006
)

// #region 建立騎乘紀錄相關

// 建立騎乘紀錄結果類別資料
const (
	//  CreateRideRecordResultFail : 建立失敗
	CreateRideRecordResultFail int = 0

	// CreateRideRecordResultSuccess : 建立成功
	CreateRideRecordResultSuccess int = 1
)

// AddRecordRequest :建立騎乘紀錄請求
type AddRecordRequest struct {
	Altitude     float64
	County       int
	Distance     float64
	Level        int
	MemberID     string
	Photo        string
	Route        string
	ShareContent string
	SharedType   int
	Time         int64
	Title        string
}

// AddRecordResponse : 建立騎乘紀錄請求結果
type AddRecordResponse struct {
	Result        int
	TotalAltitude float64
	TotalDistance float64
	TotalRideTime int64
}

// #endregion
