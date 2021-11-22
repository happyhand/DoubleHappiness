package main

import (
	"github.com/gin-gonic/gin"
)

// InitRoute : 初始化路由
func InitRoute(router *gin.Engine) {
	// api := router.Group("api")
	// {
	// 	apiCommon := api.Group("common")
	// 	{
	// 		apiCommon.GET("countymap", common.CountyMapGet)
	// 		apiCommon.GET("version", common.VersionGet)

	// 		apiCommon.POST("verificationcode", common.VerificationCodePost)
	// 	}

	// 	apiMember := api.Group("member")
	// 	{
	// 		apiMember.POST("register", member.RegisterPost)
	// 		apiMember.POST("login", member.LoginPost)

	// 		apiMember.PATCH("passwored", member.PasswordPatch)

	// 		apiMemberJwt := apiMember.Group("", middleware.ValidateJwt())
	// 		apiMemberJwt.GET("cardinfo", member.CardInfoGet)
	// 		apiMemberJwt.GET("homeinfo", member.HomeInfoGet)
	// 		apiMemberJwt.GET("keeponline", member.KeepOnlineGet)
	// 		apiMemberJwt.GET("info", member.InfoGet)
	// 		apiMemberJwt.GET("search", member.SearchGet)

	// 		apiMemberJwt.POST("password", member.PasswordPost)
	// 		apiMemberJwt.POST("mobile", member.MobileBindPost)

	// 		apiMemberJwt.PATCH("info", member.InfoPatch)
	// 	}

	// 	apiRide := api.Group("ride")
	// 	{
	// 		apiRideJwt := apiRide.Group("", middleware.ValidateJwt())
	// 		apiRideRecordJwt := apiRideJwt.Group("record")
	// 		apiRideRecordJwt.GET("", ride.RecordGet)
	// 		apiRideRecordJwt.GET("detail", ride.RecordDetailGet)

	// 		apiRideRecordJwt.POST("", ride.RecordPost)
	// 	}
	// }

}
