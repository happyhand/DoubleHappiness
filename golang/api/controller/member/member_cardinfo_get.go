package member

import (
	"fmt"
	"net/http"
	"time"

	"api/core/config"
	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/memberservice"
	"api/validator"

	"api/temp/api"
	"api/temp/redis"

	"github.com/gin-gonic/gin"
)

// ICardInfoGet : Get 參數
type ICardInfoGet struct {
	MemberID string `form:"memberID"`
}

// RCardInfoGet : 回應參數
type RCardInfoGet struct {
	Avatar     string `json:"avatar"`
	FrontCover string `json:"frontCover"`
	HasTeam    int    `json:"HasTeam"`
	MemberID   string `json:"memberID"`
	Nickname   string `json:"nickname"`
}

// CardInfoGet : 取得會員名片資訊
func CardInfoGet(gc *gin.Context) {
	var input ICardInfoGet
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

	// 設定相關參數並讀取資料
	searchID := user.MemberID
	targerID := input.MemberID
	if targerID == "" {
		targerID = user.MemberID
	}

	redisKey := fmt.Sprintf(config.MemberCardInfoRedisKey, searchID, targerID)
	result, err := redis.Get(redis.Cache, config.MemberDB, redisKey, func() (value interface{}, expiration time.Duration, err error) {
		// 無 redis 資料時，用以下方法取得資料
		getInfoParams := memberservice.GetInfoParams{
			SearchID: searchID,
			TargetID: targerID,
		}
		dto, customError := memberservice.GetInfo(getInfoParams)
		if customError != nil {
			return nil, -1, customError
		}

		return transformRCardInfoGet(*dto), config.MemberCardInfoTimeout, nil
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
func transformRCardInfoGet(dto memberservice.MemberDto) RCardInfoGet {
	return RCardInfoGet{
		Avatar:     dto.Avatar,
		FrontCover: dto.FrontCover,
		HasTeam:    dto.HasTeam,
		MemberID:   dto.MemberID,
		Nickname:   dto.Nickname,
	}
}
