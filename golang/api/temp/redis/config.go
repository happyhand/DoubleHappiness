package redis

import (
	"fmt"
	"time"

	"api/temp/config"
)

// ConnectConfig : 連線設定
type ConnectConfig struct {
	Name        string
	Connection  string
	DialTimeout time.Duration
	ReadTimeout time.Duration
	PoolSize    int
	PoolTimeout time.Duration
}

// Cache : cache 連線設定
var Cache ConnectConfig = connectMethod("CACHE")

// connectMethod : 取得連線設定
func connectMethod(redisType string) ConnectConfig {
	env := config.EnvForge()
	return ConnectConfig{
		Name:       redisType,
		Connection: env.GetString(fmt.Sprintf("REDIS_%s", redisType)),
		// 以下固定設定
		DialTimeout: 100 * time.Second,
		ReadTimeout: 100 * time.Second,
		PoolSize:    2000,
		PoolTimeout: 3000 * time.Second,
	}
}
