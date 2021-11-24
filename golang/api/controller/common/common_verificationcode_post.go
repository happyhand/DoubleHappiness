package common

import (
	"net/http"

	"api/core/customerror"
	"api/service/emailservice"
	"api/service/verificationcodeservice"

	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// IVerificationCodePost : Post 參數
type IVerificationCodePost struct {
	Email string `json:"email" binding:"required,email"`
	Type  int    `json:"type" binding:"required,oneof=1 2"` // 1:忘記密碼 2:手機綁定
}

// VerificationCodePost : 取得驗證碼
func VerificationCodePost(gc *gin.Context) {
	var input IVerificationCodePost
	err := gc.ShouldBind(&input)
	if err != nil {
		api.ResponseFail(gc, http.StatusBadRequest, customerror.InputDataError, err.Error())
		return
	}

	generateParams := verificationcodeservice.GenerateParams{
		Email: input.Email,
		Type:  input.Type,
	}
	code, customError := verificationcodeservice.Generate(generateParams)
	if customError != nil {
		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	emailservice.SendVerificationCodeEmail(input.Email, code)
	api.ResponseSuccess(gc, nil)
}
