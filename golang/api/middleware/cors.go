package middleware

import (
	"api/core/customerror"
	"api/temp/api"
	"net/http"
	"strconv"
	"strings"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/valyala/fasthttp"
)

var (
	defaultAllowHeaders = []string{"Origin", "Accept", "Content-Type", "Authorization", "ApiToken", "User-Agent", "Referer", "Pragma", "action", "App-Origin"}
	defaultAllowMethods = []string{"GET", "POST", "PUT", "DELETE", "PATCH", "HEAD"}
)

// corsOptions : stores configurations
type corsOptions struct {
	AllowOrigins     []string
	AllowCredentials bool
	AllowMethods     []string
	AllowHeaders     []string
	ExposeHeaders    []string
	MaxAge           time.Duration
}

// Cors : Middleware sets CORS headers for every request
func Cors() gin.HandlerFunc {

	corsOptions := corsOptions{}

	corsOptions.AllowCredentials = true

	if corsOptions.AllowHeaders == nil {
		corsOptions.AllowHeaders = defaultAllowHeaders
	}

	if corsOptions.AllowMethods == nil {
		corsOptions.AllowMethods = defaultAllowMethods
	}

	return func(gc *gin.Context) {
		req := gc.Request
		res := gc.Writer
		origin := req.Header.Get("Origin")
		requestMethod := req.Header.Get("Access-Control-Request-Method")
		requestHeaders := req.Header.Get("Access-Control-Request-Headers")

		if len(corsOptions.AllowOrigins) > 0 {
			res.Header().Set("Access-Control-Allow-Origin", strings.Join(corsOptions.AllowOrigins, " "))
		} else {
			res.Header().Set("Access-Control-Allow-Origin", origin)
		}

		if corsOptions.AllowCredentials {
			res.Header().Set("Access-Control-Allow-Credentials", "true")
		}

		if len(corsOptions.ExposeHeaders) > 0 {
			res.Header().Set("Access-Control-Expose-Headers", strings.Join(corsOptions.ExposeHeaders, ","))
		}

		if len(corsOptions.AllowMethods) > 0 {
			res.Header().Set("Access-Control-Allow-Methods", strings.Join(corsOptions.AllowMethods, ","))
		} else if requestMethod != "" {
			res.Header().Set("Access-Control-Allow-Methods", requestMethod)
		}

		if len(corsOptions.AllowHeaders) > 0 {
			res.Header().Set("Access-Control-Allow-Headers", strings.Join(corsOptions.AllowHeaders, ","))
		} else if requestHeaders != "" {
			res.Header().Set("Access-Control-Allow-Headers", requestHeaders)
		}

		if corsOptions.MaxAge > time.Duration(0) {
			res.Header().Set("Access-Control-Max-Age", strconv.FormatInt(int64(corsOptions.MaxAge/time.Second), 10))
		}

		if req.Method == "OPTIONS" {
			gc.AbortWithStatus(fasthttp.StatusNoContent)
		} else {
			gc.Next()
		}

		if req.Method == "GET" || req.Method == "POST" || req.Method == "PUT" || req.Method == "DELETE" || req.Method == "PATCH" || req.Method == "HEAD" {
			statusCode := gc.Writer.Status()
			if statusCode == 404 {
				gc.AbortWithStatusJSON(http.StatusNotFound, api.ResponseBody{
					ResultCode:    customerror.PageNotFound,
					ResultMessage: "404 page not found",
				})
			}

		} else {
			gc.Next()
		}
	}
}
