package member

import (
	"fmt"
	"net/http"

	"api/core/config"
	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/memberservice"
	"api/service/verificationcodeservice"
	"api/validator"

	"api/temp/api"
	"api/temp/lock"

	"github.com/gin-gonic/gin"
)

// IMobileBindPost : Post 參數
type IMobileBindPost struct {
	Mobile           string `json:"mobile" binding:"required,mobile"`
	VerificationCode string `json:"verificationCode" binding:"required"`
}

// MobileBindPost : 手機綁定
func MobileBindPost(gc *gin.Context) {
	var input IMobileBindPost
	err := gc.ShouldBind(&input)
	if err != nil {
		api.ResponseFail(gc, http.StatusBadRequest, customerror.InputDataError, validator.TransformCustomTrans(err))
		return
	}

	// 取得 jwt payload user
	user, ok := gc.MustGet(jwtservice.PayloadKey).(*jwtservice.Payload)
	if !ok {
		api.ResponseFail(gc, http.StatusBadRequest, customerror.Forbidden, "jwt payload parse error")
		return
	}

	// 驗證驗證碼
	validateParams := verificationcodeservice.ValidateParams{
		Code:  input.VerificationCode,
		Email: user.Email,
		Type:  verificationcodeservice.MobileBindType,
	}
	result, customError := verificationcodeservice.Validate(validateParams)
	if customError != nil {
		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	if !result {
		api.ResponseFail(gc, http.StatusConflict, customerror.VerificationCodeValidateFail, "illegal verification code")
		return
	}

	// 加入原子鎖
	key := fmt.Sprintf(config.LockMobileRedisKey, user.Email)
	value, err := lock.Lock(key)
	if err != nil {
		api.ResponseFail(gc, http.StatusInternalServerError, customerror.SystemError, err.Error())
		return
	}

	// 綁定驗證
	mobileBindParams := memberservice.MobileBindParams{
		MemberID: user.MemberID,
		Mobile:   input.Mobile,
	}
	customError = memberservice.MobileBind(mobileBindParams)
	lock.Unlock(key, value) // 解除原子鎖
	if customError != nil {
		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	// 刪除驗證碼
	deleteParams := verificationcodeservice.DeleteParams{
		Email: user.Email,
		Type:  verificationcodeservice.MobileBindType,
	}
	verificationcodeservice.Delete(deleteParams)

	api.ResponseSuccess(gc, nil)
}
