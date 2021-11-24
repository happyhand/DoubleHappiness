package ride

import (
	"fmt"
	"net/http"
	"time"

	"api/core/config"
	"api/core/customerror"
	"api/service/rideservice"
	"api/validator"

	"api/temp/api"
	"api/temp/redis"

	"github.com/gin-gonic/gin"
)

// IRecordDetailGet : Get 參數
type IRecordDetailGet struct {
	RideID string `form:"rideID" binding:"required"`
}

// RRecordDetailGet : 回應參數
type RRecordDetailGet struct {
	Altitude     float64 `json:"altitude"`
	County       int     `json:"county"`
	CreateDate   string  `json:"createDate"`
	Distance     float64 `json:"distance"`
	Level        int     `json:"level"`
	MemberID     string  `json:"memberID"`
	Photo        string  `json:"photo"`
	RideID       string  `json:"rideID"`
	Route        string  `json:"route"`
	ShareContent string  `json:"shareContent"`
	SharedType   int     `json:"sharedType"`
	Time         int64   `json:"time"`
	Title        string  `json:"title"`
}

// RecordDetailGet : 取得騎乘紀錄明細
func RecordDetailGet(gc *gin.Context) {
	var input IRecordDetailGet
	err := gc.ShouldBindQuery(&input)
	if err != nil {
		api.ResponseFail(gc, http.StatusBadRequest, customerror.InputDataError, validator.TransformCustomTrans(err))
		return
	}

	redisKey := fmt.Sprintf(config.RideRecordDetailRedisKey, input.RideID)
	result, err := redis.Get(redis.Cache, config.RideDB, redisKey, func() (value interface{}, expiration time.Duration, err error) {
		// 無 redis 資料時，用以下方法取得資料
		dto, customError := rideservice.GetAppointRecord(input.RideID)
		if customError != nil {
			return nil, -1, customError
		}

		return transformRRecordDetailGet(*dto), config.RideInfoTimeout, nil
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

// transformRRecordDetailGet : 轉換回應資訊
func transformRRecordDetailGet(dto rideservice.RideRecordDto) RRecordDetailGet {
	return RRecordDetailGet{
		Altitude:     dto.Altitude,
		County:       dto.County,
		CreateDate:   dto.CreateDate,
		Distance:     dto.Distance,
		Level:        dto.Level,
		MemberID:     dto.MemberID,
		Photo:        dto.Photo,
		RideID:       dto.RideID,
		Route:        dto.Route,
		ShareContent: dto.ShareContent,
		SharedType:   dto.SharedType,
		Time:         dto.Time,
		Title:        dto.Title,
	}
}
