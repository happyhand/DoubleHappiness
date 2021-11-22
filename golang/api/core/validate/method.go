package validate

import (
	"api/core/config"
	"regexp"
)

// Password : 驗證密碼格式
func Password(password string) bool {
	// ^:字首 [a-zA-Z]:第一個字符,須為英文 [\w]{5,11}:往後字符,須為a-zA-Z0-9,且不為特殊符號,並設定長度為 5-11 $:字末
	r := regexp.MustCompile(`^[a-zA-Z][\w]{5,11}$`)
	return r.MatchString(password)
}

// Nickname : 驗證暱稱格式
func Nickname(name string) bool {
	r := regexp.MustCompile(`^\S{1,6}$`)
	return r.MatchString(name)
}

// Mobile : 驗證手機格式
func Mobile(mobile string) bool {
	r := regexp.MustCompile(`^(0|\+886)9\d{8}$`)
	return r.MatchString(mobile)
}

// County : 驗證縣市
func County(county int) bool {
	_, exist := config.CountyMap[county]
	return exist
}
