package member

import (
	"net/http"

	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/memberservice"
	"api/validator"

	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// INotifyTokenPost : Post 參數
type INotifyTokenPost struct {
	NotifyToken string `json:"notifyToken" binding:"required"`
}

// NotifyTokenPost : 會員更新 notify token
func NotifyTokenPost(gc *gin.Context) {
	var input INotifyTokenPost
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

	// 更新 notify token
	updateNotifyTokenParams := memberservice.UpdateNotifyTokenParams{
		MemberID:    user.MemberID,
		NotifyToken: input.NotifyToken,
	}
	customError := memberservice.UpdateNotifyToken(updateNotifyTokenParams)
	if customError != nil {
		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	// 回應客端
	api.ResponseSuccess(gc, user)
}
