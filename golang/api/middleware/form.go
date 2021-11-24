package middleware

import (
	"net/http"

	"api/core/customerror"

	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// Form : check parse form
func Form() gin.HandlerFunc {
	return func(gc *gin.Context) {
		var err error
		if gc.ContentType() == "multipart/form-data" {
			err = gc.Request.ParseMultipartForm(32 << 20)
		} else {
			err = gc.Request.ParseForm()
		}

		if err != nil {
			gc.AbortWithStatusJSON(http.StatusBadRequest, api.ResponseBody{
				ResultCode:    customerror.InputParseError,
				ResultMessage: "input parse error",
			})
			return
		}

		gc.Next()
	}
}
