package email

import (
	"api/temp/config"
)

// Setting : 設定資料
type Setting struct {
	Company  string
	Account  string
	Password string
	SMTPHost string
	SMTPPort string
}

// setting : email 設定
var setting Setting = readConfig()

// readConfig : 取得設定
func readConfig() Setting {
	env := config.EnvForge()
	return Setting{
		Company:  env.GetString("EMAIL_COMPANY"),
		Account:  env.GetString("EMAIL_COMPANY_ACCOUNT"),
		Password: env.GetString("EMAIL_COMPANY_PASSWORD"),
		// 以下固定設定
		SMTPHost: "smtp.gmail.com",
		SMTPPort: "587",
	}
}
