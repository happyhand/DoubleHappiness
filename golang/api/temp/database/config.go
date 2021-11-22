package database

import (
	"fmt"

	"api/temp/config"
)

// ConnectConfig : 連線設定
type ConnectConfig struct {
	Account      string
	Password     string
	IP           string
	Port         string
	DB           string
	Charset      string
	Location     string
	Timeout      string
	MaxLifeTime  int
	MaxOpenConns int
	MaxIdleConns int
}

var (
	// Master : master 連線設定
	Master ConnectConfig = connectMethod("MASTER")
	// Slave : slave 連線設定
	Slave ConnectConfig = connectMethod("SLAVE")
)

// connectMethod : 取得連線設定
func connectMethod(dbType string) ConnectConfig {
	env := config.EnvForge()
	return ConnectConfig{
		Account:  env.GetString(fmt.Sprintf("DB_%s_ACCOUNT", dbType)),
		Password: env.GetString(fmt.Sprintf("DB_%s_PASSWORD", dbType)),
		IP:       env.GetString(fmt.Sprintf("DB_%s_IP", dbType)),
		Port:     env.GetString(fmt.Sprintf("DB_%s_PORT", dbType)),
		DB:       env.GetString("DB_DATABASE"),
		// 以下固定設定
		Charset:      "utf8mb4",
		Location:     "UTC",
		Timeout:      "60s",
		MaxLifeTime:  900,
		MaxOpenConns: 500,
		MaxIdleConns: 500,
	}
}
