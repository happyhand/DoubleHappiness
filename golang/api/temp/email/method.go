package email

import (
	"fmt"
	"net/smtp"

	"github.com/jordan-wright/email"
)

// SendEmail : 發送郵件
func SendEmail(toEmail []string, subject string, body string) error {
	e := email.NewEmail()
	e.From = fmt.Sprintf("%s <%s>", setting.Company, setting.Account)
	e.To = toEmail
	e.Subject = subject
	e.HTML = []byte(body)
	return e.Send(fmt.Sprintf("%s:%s", setting.SMTPHost, setting.SMTPPort), smtp.PlainAuth(setting.Company, setting.Account, setting.Password, setting.SMTPHost))
}
