package emailservice

import (
	"fmt"

	"api/core/config"

	"api/temp/ants"
	"api/temp/email"
	"api/temp/log"
)

// SendVerificationCodeEmail : 發送驗證碼 email
func SendVerificationCodeEmail(userEmail string, code string) {
	ants.Do(nil, func() {
		body := "<p>親愛的用戶您好</p>"
		body += "<p>您的驗證碼為</p>"
		body += fmt.Sprintf("<p><span style='font-weight:bold; color:blue;'>%s</span></p>", code)
		body += fmt.Sprintf("<p>請於 <span style='font-weight:bold; color:blue;'>%.f分鐘</span> 內於APP輸入此驗證碼</p>", config.VerificationCodeTimeout.Minutes()) // %9.2f: 浮點數小數點前９位後２位
		body += "<br><br><br>"
		body += "<p>※本電子郵件係由系統自動發送，請勿直接回覆本郵件。</p>"
		err := email.SendEmail([]string{userEmail}, "GoBike 驗證碼通知信", body)
		if err != nil {
			log.Error("send verification code email fail", err, map[string]interface{}{
				"email": userEmail,
			})
		}
	})
}
