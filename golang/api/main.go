package main

import (
	"api/middleware"
	"api/validator"
	"fmt"

	limit "github.com/aviddiviner/gin-limit"
	"github.com/gin-gonic/gin"
)

func main() {
	fmt.Println("[Api Server Start]")
	gin.SetMode(gin.ReleaseMode)
	g := gin.New()

	// 註冊middleware
	g.Use(limit.MaxAllowed(20000)) // 限制最大連線數
	g.Use(middleware.Cors())       // 設定 cors
	g.Use(gin.Recovery())
	// 註冊驗證器
	validator.GetInstance().Register()
	// 初始化路由
	InitRoute(g)
	_ = g.Run(":18600")
}
