package main

import (
	"api/controller/common"
	"api/controller/echo"
	"api/controller/member"
	"api/controller/ride"
	"api/middleware"

	"github.com/gin-gonic/gin"
)

// InitRoute : 初始化路由
func InitRoute(router *gin.Engine) {
	router.POST("echo", echo.Post)

	api := router.Group("api")
	{
		apiCommon := api.Group("common")
		{
			apiCommon.GET("countymap", common.CountyMapGet)
			apiCommon.GET("version", common.VersionGet)

			apiCommon.POST("verificationcode", common.VerificationCodePost)
		}

		apiMember := api.Group("member")
		{

			apiMemberJwt := apiMember.Group("", middleware.ValidateJwt())
			apiMember.POST("login", member.LoginPost)
			apiMemberJwt.GET("cardinfo", member.CardInfoGet)
			apiMemberJwt.GET("homeinfo", member.HomeInfoGet)
			apiMemberJwt.GET("keeponline", member.KeepOnlineGet)
			apiMemberJwt.GET("info", member.InfoGet)
			apiMemberJwt.GET("search", member.SearchGet)
			apiMemberJwt.POST("mobile", member.MobileBindPost)
			apiMemberJwt.PATCH("info", member.InfoPatch)
		}

		apiRide := api.Group("ride")
		{
			apiRideJwt := apiRide.Group("", middleware.ValidateJwt())
			apiRideRecordJwt := apiRideJwt.Group("record")
			apiRideRecordJwt.GET("", ride.RecordGet)
			apiRideRecordJwt.GET("detail", ride.RecordDetailGet)

			apiRideRecordJwt.POST("", ride.RecordPost)
		}
	}

}
