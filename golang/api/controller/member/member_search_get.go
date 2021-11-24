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

// ISearchGet : Get 參數
type ISearchGet struct {
	SearchKey      string `form:"searchKey" binding:"required"`
	UseFuzzySearch int    `form:"useFuzzySearch" binding:"omitempty,oneof=0 1"`
}

// RSearchGet : Get 回應參數
type RSearchGet struct {
	Avatar     string `json:"avatar"`
	MemberID   string `json:"memberID"`
	Nickname   string `json:"nickname"`
	OnlineType int    `json:"onlineType"`
}

// SearchGet : 搜尋會員
func SearchGet(gc *gin.Context) {
	var input ISearchGet
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
	redisKey := fmt.Sprintf(config.MemberSearchRedisKey, input.SearchKey, input.UseFuzzySearch, searchID)
	result, err := redis.Get(redis.Cache, config.MemberDB, redisKey, func() (value interface{}, expiration time.Duration, err error) {
		// 無 redis 資料時，用以下方法取得資料
		searchParams := memberservice.SearchParams{
			Key:               input.SearchKey,
			MemberID:          user.MemberID,
			IsFuzzy:           input.UseFuzzySearch == 1,
			CheckSearchStatus: false, // TODO 待確認是否要檢查可被搜尋
		}
		dtos, customError := memberservice.Search(searchParams)
		if customError != nil {
			return nil, -1, customError
		}

		var response []RSearchGet
		for _, dto := range dtos {
			response = append(response, transformRSearchGet(dto))
		}

		return response, config.MemberSearchTimeout, nil
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

// transformRSearchGet : 轉換回應資訊
func transformRSearchGet(dto memberservice.MemberDto) RSearchGet {
	return RSearchGet{
		Avatar:     dto.Avatar,
		MemberID:   dto.MemberID,
		Nickname:   dto.Nickname,
		OnlineType: dto.OnlineType,
	}
}
