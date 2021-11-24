package customerror

import "net/http"

const (
	// Unauthorized : 未授權
	Unauthorized int = 10401
	// Forbidden : 拒絕訪問
	Forbidden int = 10403
	// PageNotFound : 頁面不存在
	PageNotFound int = 10404
	// SystemError : 系統發生錯誤，請稍後再試
	SystemError int = 10500

	// InputParseError : 解析參數錯誤
	InputParseError int = 20001
	// InputDataError : 請求參數錯誤
	InputDataError int = 20002
	// UploadFileError : 上傳檔案錯誤
	UploadFileError int = 20003
	// InsertDataError : 新增資料錯誤
	InsertDataError int = 20004
	// UpdateDataError : 更新資料錯誤
	UpdateDataError int = 20005
	// DataNotExistError : 資料不存在
	DataNotExistError int = 20006
	// DataRepeatFail : 資料重複，請重新輸入
	DataRepeatFail int = 20007
	// PrivateDataError : 無法取得非公開資料
	PrivateDataError int = 20008

	// RegisterFail : 系統拒絕註冊，請稍後再試
	RegisterFail int = 21001
	// LoginFail : 系統拒絕登入，請稍後再試
	LoginFail int = 21002
	// EmailOrPasswordNotMatch : 信箱與密碼不符合，請重新輸入
	EmailOrPasswordNotMatch int = 21003
	// IllegalEmail : 信箱錯誤，請重新輸入
	IllegalEmail int = 21004
	// VerificationCodeGenerateFail : 產生驗證碼失敗，請等待 m 分鐘後再試
	VerificationCodeGenerateFail int = 21005
	// VerificationCodeValidateFail : 驗證碼錯誤，請重新輸入
	VerificationCodeValidateFail int = 21006
	// MobileBind : 已完成綁定手機，請勿重複綁定
	MobileBind int = 21007
)

// Error : 自定義 error
type Error struct {
	Code    int
	Message string
}

// New : 建立 Error
func New(code int, message string) *Error {
	return &Error{
		Code:    code,
		Message: message,
	}
}

// Error : 錯誤資訊
func (e *Error) Error() string {
	return e.Message
}

// HTTPStatusCode : 取得對應 http status code
func (e *Error) HTTPStatusCode() int {
	switch e.Code {
	case InputParseError,
		InputDataError,
		UploadFileError,
		InsertDataError,
		UpdateDataError,
		DataNotExistError,
		DataRepeatFail,
		PrivateDataError:
		return http.StatusBadRequest
	case Unauthorized:
		return http.StatusUnauthorized
	case Forbidden:
		return http.StatusForbidden
	case PageNotFound:
		return http.StatusNotFound
	case SystemError:
		return http.StatusInternalServerError
	default:
		return http.StatusConflict
	}
}
