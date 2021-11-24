package echo

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

// Post : return echo response
func Post(gc *gin.Context) {
	var input interface{}
	err := gc.ShouldBind(&input)
	if err != nil {
		gc.JSON(http.StatusBadRequest, err)
		return
	}

	gc.JSON(http.StatusOK, input)
}
