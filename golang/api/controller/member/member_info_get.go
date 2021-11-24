package member

import (
	"fmt"
	"net/http"
	"time"

	"api/core/config"
	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/memberservice"

	"api/temp/api"
	"api/temp/redis"

	"github.com/gin-gonic/gin"
)

// RInfoGet : 回應參數
type RInfoGet struct {
	Birthday   string  `json:"birthday"`
	BodyHeight float64 `json:"bodyHeight"`
	BodyWeight float64 `json:"bodyWeight"`
	FrontCover string  `json:"frontCover"`
	Gender     int     `json:"gender"`
	HasMobile  int     `json:"hasMobile"`
	Mobile     string  `json:"mobile"`
}

// InfoGet : 取得會員名片資訊
func InfoGet(gc *gin.Context) {
	// 取得 jwt payload user
	user, ok := gc.MustGet(jwtservice.PayloadKey).(*jwtservice.Payload)
	if !ok {
		api.ResponseFail(gc, http.StatusBadRequest, customerror.Forbidden, "jwt payload parse error")
		return
	}

	// 設定相關參數並讀取資料
	redisKey := fmt.Sprintf(config.MemberInfoRedisKey, user.MemberID)
	result, err := redis.Get(redis.Cache, config.MemberDB, redisKey, func() (value interface{}, expiration time.Duration, err error) {
		// 無 redis 資料時，用以下方法取得資料
		getInfoParams := memberservice.GetInfoParams{
			SearchID: user.MemberID,
			TargetID: user.MemberID,
		}
		dto, customError := memberservice.GetInfo(getInfoParams)
		if customError != nil {
			return nil, -1, customError
		}

		return transformRInfoGet(*dto), config.MemberInfoTimeout, nil
	})

	if err != nil {
		customError, ok := err.(*customerror.Error)
		if ok {
			api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
			return
		}

		api.ResponseFail(gc, http.StatusInternalServerError, customerror.SystemError, err.Error())
		return
	}

	api.ResponseSuccess(gc, result)
}

// transformRCardInfoGet : 轉換回應資訊
func transformRInfoGet(dto memberservice.MemberDto) RInfoGet {
	var hasMobile int = 0
	if dto.Mobile != "" {
		hasMobile = 1
	}
	return RInfoGet{
		Birthday:   dto.Birthday,
		BodyHeight: dto.BodyHeight,
		BodyWeight: dto.BodyWeight,
		FrontCover: dto.FrontCover,
		Gender:     dto.Gender,
		HasMobile:  hasMobile,
		Mobile:     dto.Mobile,
	}
}
