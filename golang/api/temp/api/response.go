package api

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

// ResponseBody : 回覆內容
type ResponseBody struct {
	Result        bool        `json:"result"`
	ResultCode    int         `json:"resultCode"`
	ResultMessage string      `json:"resultMessage"`
	Content       interface{} `json:"content"`
}

// ResponseSuccess : api 回覆成功
func ResponseSuccess(gc *gin.Context, data interface{}) {
	gc.JSON(http.StatusOK, ResponseBody{
		Result:     true,
		ResultCode: 1,
		Content:    data,
	})
}

// ResponseFail : api 回覆失敗
func ResponseFail(gc *gin.Context, statusCode int, resultCode int, message string) {
	gc.JSON(statusCode, ResponseBody{
		Result:        false,
		ResultCode:    resultCode,
		ResultMessage: message,
	})
}
