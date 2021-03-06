package model

// ResponseMessage :: Api 回應資料
type ResponseMessage struct {
	Result        bool        `json:"result"`
	ResultCode    int         `json:"resultCode"`
	ResultMessage string      `json:"resultMessage"`
	Content       interface{} `json:"content"`
}

const (
	// InputInvalid :: Api Response Message - 無效的請求內容
	InputInvalid string = "InputInvalid"
	// SystemError :: Api Response Message - 系統錯誤
	SystemError string = "SystemError"
)
