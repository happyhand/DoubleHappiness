package verificationcodeservice

const (
	// ForgotPasswordType : 忘記密碼類型
	ForgotPasswordType int = 1 + iota
	// MobileBindType : 手機綁定類型
	MobileBindType
)

// GenerateParams : 生成驗證碼參數內容
type GenerateParams struct {
	Email string
	Type  int
}

// ValidateParams : 驗證驗證碼參數內容
type ValidateParams struct {
	Code  string
	Email string
	Type  int
}

// DeleteParams : 刪除驗證碼參數內容
type DeleteParams struct {
	Email string
	Type  int
}
