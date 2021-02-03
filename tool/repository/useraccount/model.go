package useraccount

const (
	// TableName : 資料表名稱
	TableName string = "useraccount"
)

// User : 使用者資料表
type User struct {
	MemberID       string
	Email          string
	Password       string
	FBToken        string
	GoogleToken    string
	NotifyToken    string
	RegisterSource int
	RegisterDate   string
}
