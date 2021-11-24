package ride

import (
	"mime/multipart"
	"net/http"
	"strconv"

	"api/core/customerror"
	"api/service/jwtservice"
	"api/service/rideservice"
	"api/service/uploadservice"
	"api/validator"

	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// IRecordPost : Post 參數
type IRecordPost struct {
	Altitude     string                  `form:"altitude"`
	County       int                     `form:"county" binding:"required,county"`
	Distance     string                  `form:"distance"`
	Level        int                     `form:"level"`
	Photo        *multipart.FileHeader   `form:"photo" binding:"required"`
	Route        string                  `form:"route" binding:"required"`
	ShareContent []string                `form:"shareContent"`
	ShareImage   []*multipart.FileHeader `form:"shareImage"`
	SharedType   int                     `form:"sharedType"`
	Time         string                  `form:"time"`
	Title        string                  `form:"title"`
}

// RecordPost : 新增騎乘紀錄
func RecordPost(gc *gin.Context) {
	var input IRecordPost
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
	altitude, _ := strconv.ParseFloat(input.Altitude, 64)
	distance, _ := strconv.ParseFloat(input.Distance, 64)
	time, _ := strconv.ParseInt(input.Time, 10, 64)
	params := rideservice.AddRecordParams{
		Altitude:     altitude,
		County:       input.County,
		Distance:     distance,
		Level:        input.Level,
		MemberID:     user.MemberID,
		Route:        input.Route,
		ShareContent: input.ShareContent,
		SharedType:   input.SharedType,
		Time:         time,
		Title:        input.Title,
	}

	// 上傳圖片
	filename, customError := uploadservice.UploadImage(input.Photo, user.MemberID, "ride")
	if customError != nil {
		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	params.Photo = filename

	images := []string{}
	for _, image := range input.ShareImage {
		filename, customError := uploadservice.UploadImage(image, user.MemberID, "ride")
		if customError != nil {
			api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
			return
		}

		images = append(images, filename)
	}

	params.ShareImage = images
	// 新增騎乘紀錄
	customError = rideservice.AddRecord(params)
	if customError != nil {
		// 更新失敗，刪除已成功上傳的圖片
		uploadservice.DeleteImage(params.Photo, user.MemberID, "ride")
		for _, image := range images {
			uploadservice.DeleteImage(image, user.MemberID, "ride")
		}

		api.ResponseFail(gc, customError.HTTPStatusCode(), customError.Code, customError.Message)
		return
	}

	api.ResponseSuccess(gc, nil)
}
