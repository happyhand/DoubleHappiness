package common

import (
	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// VersionGet : 取得取得版本號
func VersionGet(gc *gin.Context) {
	api.ResponseSuccess(gc, "1.0.0.0")
}
