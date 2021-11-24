package rideservice

// AddRecordParams : 新增騎乘紀錄參數內容
type AddRecordParams struct {
	Altitude     float64
	County       int
	Distance     float64
	Level        int
	MemberID     string
	Photo        string
	Route        string
	ShareContent []string
	ShareImage   []string
	SharedType   int
	Time         int64
	Title        string
}

// RideRecordDto : 騎乘紀錄資訊
type RideRecordDto struct {
	Altitude     float64
	County       int
	CreateDate   string
	Distance     float64
	Level        int
	MemberID     string
	Photo        string
	RideID       string
	Route        string
	ShareContent string
	SharedType   int
	Time         int64
	Title        string
}

// RideDistanceDto : 騎乘距離資訊
type RideDistanceDto struct {
	MemberID      string
	TotalDistance float64
	WeekDistance  float64
}
