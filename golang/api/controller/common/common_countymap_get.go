package common

import (
	"api/core/config"

	"api/temp/api"

	"github.com/gin-gonic/gin"
)

// CountyMapGet : 取得市區資料列表
func CountyMapGet(gc *gin.Context) {
	api.ResponseSuccess(gc, config.CountyMap)
}
