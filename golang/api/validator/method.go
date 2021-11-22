package validator

import (
	"api/core/validate"
	"api/temp/log"
	"strings"
	"sync"

	"github.com/gin-gonic/gin/binding"
	"github.com/go-playground/locales/en"
	ut "github.com/go-playground/universal-translator"
	"github.com/go-playground/validator/v10"
	en_translations "github.com/go-playground/validator/v10/translations/en"
)

// Validator : 驗證器
type Validator struct {
	validate *validator.Validate
	trans    ut.Translator
}

var instance *Validator
var once sync.Once

// GetInstance : 取得驗證器
func GetInstance() *Validator {
	once.Do(func() {
		enTranslator := en.New()
		utT := ut.New(enTranslator, enTranslator)
		trans, found := utT.GetTranslator("en")
		if !found {
			log.Error("not found translator", nil, nil)
			return
		}

		v := binding.Validator.Engine().(*validator.Validate)
		err := en_translations.RegisterDefaultTranslations(v, trans)
		if err != nil {
			log.Error("register default translations fail", err, nil)
			return
		}

		instance = &Validator{
			validate: v,
			trans:    trans,
		}
	})

	return instance
}

// TransformCustomTrans : 轉換自定義驗證錯誤
func TransformCustomTrans(err error) string {
	e, ok := err.(validator.ValidationErrors)
	if ok {
		return e[0].Translate(instance.trans)
	}

	return err.Error()
}

// Register : 註冊所有驗證
func (v *Validator) Register() {
	v.registerCustom("password", "illegal password fromat", validatePassword())
	v.registerCustom("nickname", "illegal nickname fromat", validateNickname())
	v.registerCustom("mobile", "illegal mobile fromat", validateMobile())
	v.registerCustom("county", "illegal county", validateCounty())
}

// registerCustom : 註冊自定義驗證
func (v *Validator) registerCustom(tag string, message string, doFunc func(fl validator.FieldLevel) bool) {
	err := v.validate.RegisterTranslation(tag, v.trans, func(ut ut.Translator) error {
		err := ut.Add(tag, message, true) // see universal-translator for details
		return err
	}, func(ut ut.Translator, fe validator.FieldError) string {
		t, _ := ut.T(tag, fe.Field())
		return t
	})

	if err != nil {
		log.Error("register custom translation fail", err, map[string]interface{}{
			"tag": tag,
		})
		return
	}

	err = v.validate.RegisterValidation(tag, doFunc)

	if err != nil {
		log.Error("register custom validation fail", err, map[string]interface{}{
			"tag": tag,
		})
		return
	}
}

// registerPassword : 密碼驗證
func validatePassword() func(fl validator.FieldLevel) bool {
	return func(fl validator.FieldLevel) bool {
		password := fl.Field().String()
		return validate.Password(password)
	}
}

// validateNickname : 暱稱驗證
func validateNickname() func(fl validator.FieldLevel) bool {
	return func(fl validator.FieldLevel) bool {
		nickname := fl.Field().String()
		return validate.Nickname(nickname)
	}
}

// validateMobile : 手機驗證
func validateMobile() func(fl validator.FieldLevel) bool {
	return func(fl validator.FieldLevel) bool {
		mobile := fl.Field().String()
		result := validate.Mobile(mobile)
		if !result {
			return false
		}

		mobile = strings.Replace(mobile, "+886", "0", 1)
		fl.Field().SetString(mobile)
		return true
	}
}

// validateCounty : 縣市驗證
func validateCounty() func(fl validator.FieldLevel) bool {
	return func(fl validator.FieldLevel) bool {
		county := int(fl.Field().Int())
		return validate.County(county)
	}
}
