package riderepo

// RideDistanceDao : 騎乘距離資料
type RideDistanceDao struct {
	MemberID      string  `db:"MemberID" joindb:"rd.MemberID"`           // 會員 ID
	TotalDistance float64 `db:"TotalDistance" joindb:"rd.TotalDistance"` // 總里程
	WeekDistance  float64 `db:"WeekDistance" joindb:"wrd.WeekDistance"`  // 週里程
}

// RideRecordDao : 騎乘紀錄資料
type RideRecordDao struct {
	Altitude     float64 `db:"Altitude" joindb:"rr.Altitude"`         // 騎乘坡度
	County       int     `db:"County" joindb:"rr.County"`             // 居住地
	CreateDate   string  `db:"CreateDate" joindb:"rr.CreateDate"`     // 建立日期
	Distance     float64 `db:"Distance" joindb:"rr.Distance"`         // 騎乘距離
	Level        int     `db:"Level" joindb:"rr.Level"`               // 等級
	MemberID     string  `db:"MemberID" joindb:"rr.MemberID"`         // 會員 ID
	Photo        string  `db:"Photo" joindb:"rr.Photo"`               // 封面圖片
	RideID       string  `db:"RideID" joindb:"rr.RideID"`             // 騎乘 ID
	Route        string  `db:"Route" joindb:"rr.Route"`               // 騎乘路線
	ShareContent string  `db:"ShareContent" joindb:"rr.ShareContent"` // 分享內容
	SharedType   int     `db:"SharedType" joindb:"rr.SharedType"`     // 分享類型
	Time         int64   `db:"Time" joindb:"rr.Time"`                 // 騎乘時間
	Title        string  `db:"Title" joindb:"rr.Title"`               // 標題
}
