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

// ILoginPost : Post 參數
type ILoginPost struct {
	Avatar      string `json:"avatar"`
	Email       string `json:"email" binding:"omitempty,email"`
	LoginSource int    `json:"loginSource"`
	NickName    string `json:"nickName"`
	Token       string `json:"token"`
}

// LoginPost : 會員登入
func LoginPost(gc *gin.Context) {
	var input ILoginPost
	err := gc.ShouldBind(&input)
	if err != nil {
		api.ResponseFail(gc, http.StatusBadRequest, customerror.InputDataError, validator.TransformCustomTrans(err))
		return
	}

	// 登入驗證
	loginParams := memberservice.LoginParams{
		Avatar:      input.Avatar,
		Email:       input.Email,
		LoginSource: input.LoginSource,
		NickName:    input.NickName,
		Token:       input.Token,
	}
	dto, customError := memberservice.Login(loginParams)
	if customError != nil {
		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	// 生成 jwt token
	payload := jwtservice.Payload{
		Avatar:     dto.Avatar,
		Email:      dto.Email,
		FrontCover: dto.FrontCover,
		MemberID:   dto.MemberID,
		Nickname:   dto.Nickname,
		Photo:      dto.Photo,
	}
	token, customError := jwtservice.Generate(payload)
	if err != nil {
		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	// 更新會員最新登入時間
	memberservice.UpdateLastLoginDate(dto.MemberID)
	// 回應客端
	api.ResponseSuccess(gc, token)
}
