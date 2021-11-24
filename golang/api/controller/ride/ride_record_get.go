package ride

import (
	"fmt"
	"net/http"
	"time"

	"api/core/config"
	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/rideservice"
	"api/validator"

	"api/temp/api"
	"api/temp/redis"

	"github.com/gin-gonic/gin"
)

// IRecordGet : Get 參數
type IRecordGet struct {
	MemberID string `form:"memberID"`
}

// RRecordGet : 回應參數
type RRecordGet struct {
	Altitude   float64 `json:"altitude"`
	County     int     `json:"county"`
	CreateDate string  `json:"createDate"`
	Distance   float64 `json:"distance"`
	Level      int     `json:"level"`
	RideID     string  `json:"rideID"`
	Time       int64   `json:"time"`
	Title      string  `json:"title"`
}

// RecordGet : 取得騎乘紀錄
func RecordGet(gc *gin.Context) {
	var input IRecordGet
	err := gc.ShouldBindQuery(&input)
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

	memberID := input.MemberID
	if memberID == "" {
		memberID = user.MemberID
	}

	redisKey := fmt.Sprintf(config.RideRecordRedisKey, memberID)
	result, err := redis.Get(redis.Cache, config.RideDB, redisKey, func() (value interface{}, expiration time.Duration, err error) {
		// 無 redis 資料時，用以下方法取得資料
		dtos, customError := rideservice.GetRecord(memberID)
		if customError != nil {
			return nil, -1, customError
		}

		var response []RRecordGet
		for _, dto := range dtos {
			response = append(response, transformRRecordGet(dto))
		}

		return response, config.RideInfoTimeout, nil
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

// transformRRecordGet : 轉換回應資訊
func transformRRecordGet(dto rideservice.RideRecordDto) RRecordGet {
	return RRecordGet{
		Altitude:   dto.Altitude,
		County:     dto.County,
		CreateDate: dto.CreateDate,
		Distance:   dto.Distance,
		Level:      dto.Level,
		RideID:     dto.RideID,
		Time:       dto.Time,
		Title:      dto.Title,
	}
}
