package member

import (
	"mime/multipart"
	"net/http"
	"time"

	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/memberservice"
	"api/service/uploadservice"
	"api/validator"

	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// IInfoPatch : Patch 參數
type IInfoPatch struct {
	Avatar     *multipart.FileHeader `form:"avatar"`
	Birthday   *time.Time            `form:"birthday"` // ex:"2020-01-02T10:00:00+08:00" (time.RFC3339) TODO:待確認要不要統一時間格式
	BodyHeight *float64              `form:"bodyHeight"`
	BodyWeight *float64              `form:"bodyWeight"`
	County     *int                  `form:"county" binding:"omitempty,county"`
	FrontCover *multipart.FileHeader `form:"frontCover"`
	Gender     *int                  `form:"gender"`
	Nickname   *string               `form:"nickname" binding:"omitempty,nickname"`
	Photo      *multipart.FileHeader `form:"photo"`
}

// InfoPatch : 會員編輯資訊
func InfoPatch(gc *gin.Context) {
	var input IInfoPatch
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

	// 更新參數
	params := memberservice.EditInfoParams{
		MemberID:   user.MemberID,
		Birthday:   input.Birthday,
		BodyHeight: input.BodyHeight,
		BodyWeight: input.BodyWeight,
		County:     input.County,
		Gender:     input.Gender,
		Nickname:   input.Nickname,
	}

	// 上傳圖片
	if input.Avatar != nil {
		filename, customError := uploadservice.UploadImage(input.Avatar, user.MemberID)
		if customError != nil {
			api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
			return
		}

		params.Avatar = &filename
	}

	if input.FrontCover != nil {
		filename, customError := uploadservice.UploadImage(input.FrontCover, user.MemberID)
		if customError != nil {
			api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
			return
		}

		params.FrontCover = &filename
	}

	if input.Photo != nil {
		filename, customError := uploadservice.UploadImage(input.Photo, user.MemberID)
		if customError != nil {
			api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
			return
		}

		params.Photo = &filename
	}

	// 更新會員資訊
	oldDto, dto, customError := memberservice.EditInfo(params)
	if customError != nil {
		// 更新失敗，刪除已成功上傳的圖片
		if params.Avatar != nil {
			uploadservice.DeleteImage(*params.Avatar, user.MemberID)
		}

		if params.FrontCover != nil {
			uploadservice.DeleteImage(*params.FrontCover, user.MemberID)
		}

		if params.Photo != nil {
			uploadservice.DeleteImage(*params.Photo, user.MemberID)
		}

		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	// 更新成功，刪除舊圖片
	if params.Avatar != nil {
		uploadservice.DeleteImage(oldDto.Avatar, user.MemberID)
	}

	if params.FrontCover != nil {
		uploadservice.DeleteImage(oldDto.FrontCover, user.MemberID)
	}

	if params.Photo != nil {
		uploadservice.DeleteImage(oldDto.Photo, user.MemberID)
	}

	// 刪除會員相關 redis
	memberservice.ClearRedis(user.MemberID)

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

	// 回應客端
	api.ResponseSuccess(gc, token)
}
