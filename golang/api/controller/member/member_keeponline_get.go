package member

import (
	"net/http"

	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/memberservice"

	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// KeepOnlineGet : 會員保持在線
func KeepOnlineGet(gc *gin.Context) {
	// 取得 jwt payload user
	user, ok := gc.MustGet(jwtservice.PayloadKey).(*jwtservice.Payload)
	if !ok {
		api.ResponseFail(gc, http.StatusBadRequest, customerror.Forbidden, "jwt payload parse error")
		return
	}

	// 更新在線狀態
	memberservice.UpdateLastLoginDate(user.MemberID)
	api.ResponseSuccess(gc, nil)
}
