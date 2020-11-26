package model

// SignInContent :: 會員登入請求資料
type SignInContent struct {
	Account  string
	Password string
}

// SignInResponse :: 會員登入回應資料
type SignInResponse struct {
	Account  string `json:"account"`
	Password string `json:"password"`
}
