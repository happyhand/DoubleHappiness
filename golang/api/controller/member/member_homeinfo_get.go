package member

import (
	"fmt"
	"net/http"
	"time"

	"api/core/config"
	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/memberservice"
	"api/service/rideservice"

	"api/temp/api"
	"api/temp/redis"

	"github.com/gin-gonic/gin"
)

// RHomeInfoGet : 回應參數
type RHomeInfoGet struct {
	Avatar        string  `json:"avatar"`
	Nickname      string  `json:"nickname"`
	Photo         string  `json:"photo"`
	TotalDistance float64 `json:"totalDistance"`
}

// HomeInfoGet : 取得會員名片資訊
func HomeInfoGet(gc *gin.Context) {
	// 取得 jwt payload user
	user, ok := gc.MustGet(jwtservice.PayloadKey).(*jwtservice.Payload)
	if !ok {
		api.ResponseFail(gc, http.StatusBadRequest, customerror.Forbidden, "jwt payload parse error")
		return
	}

	// 設定相關參數並讀取資料
	redisKey := fmt.Sprintf(config.MemberHomeInfoRedisKey, user.MemberID)
	result, err := redis.Get(redis.Cache, config.MemberDB, redisKey, func() (value interface{}, expiration time.Duration, err error) {
		// 無 redis 資料時，用以下方法取得資料

		// 取得會員資訊
		getInfoParams := memberservice.GetInfoParams{
			SearchID: user.MemberID,
			TargetID: user.MemberID,
		}
		memberDto, customError := memberservice.GetInfo(getInfoParams)
		if customError != nil {
			return nil, -1, customError
		}

		// 取得騎乘距離資訊
		rideDistanceDto, customError := rideservice.GetDistance(user.MemberID)
		if customError != nil {
			return nil, -1, customError
		}

		return transformRHomeInfoGet(memberDto, rideDistanceDto), config.MemberHomeInfoTimeout, nil
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

// transformRHomeInfoGet : 轉換回應資訊
func transformRHomeInfoGet(memberDto *memberservice.MemberDto, rideDistanceDto *rideservice.RideDistanceDto) RHomeInfoGet {
	response := RHomeInfoGet{
		Avatar:   memberDto.Avatar,
		Nickname: memberDto.Nickname,
		Photo:    memberDto.Photo,
	}

	if rideDistanceDto != nil {
		response.TotalDistance = rideDistanceDto.TotalDistance
	}

	return response
}
