package middleware

import (
	"net/http"
	"strings"

	"api/core/customerror"
	"api/service/jwtservice"

	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// ValidateJwt : 驗證 jwt token
func ValidateJwt() gin.HandlerFunc {
	return func(gc *gin.Context) {
		auth := gc.GetHeader("Authorization")
		if auth == "" {
			gc.AbortWithStatusJSON(http.StatusUnauthorized, api.ResponseBody{
				ResultCode:    customerror.Unauthorized,
				ResultMessage: "illegal jwt token",
			})
			return
		}

		token := strings.Split(auth, "Bearer ")[1]
		payload, customError := jwtservice.Validate(token)
		if customError != nil {
			gc.AbortWithStatusJSON(customError.HTTPStatusCode(), api.ResponseBody{
				ResultCode:    customError.Code,
				ResultMessage: customError.Message,
			})
			return

		}

		gc.Set(jwtservice.PayloadKey, payload)
		gc.Next()
	}
}
