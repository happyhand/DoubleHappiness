package api

// Response : api 回應資料
type Response struct {
	Content       interface{} `json:"content"`
	Result        bool        `json:"result"`
	ResultCode    int         `json:"resultCode"`
	ResultMessage string      `json:"resultMessage"`
}
