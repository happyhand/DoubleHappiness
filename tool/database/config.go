package database

import (
	"time"

	"tool/config"
)

var (
	// MasterConfig : master config
	MasterConfig map[string]interface{} = connectMethod(connectConfig{
		account:  "DB_MASTER_ACCOUNT",
		password: "DB_MASTER_PASSWORD",
		ip:       "DB_MASTER_IP",
		port:     "DB_MASTER_PORT",

		db:           "gobike",
		charset:      "utf8mb4",
		location:     "UTC",
		timeout:      "60s",
		maxLifeTime:  900,
		maxOpenConns: 500,
		maxIdleConns: 500,
	})

	// SlaveConfig : slave config
	SlaveConfig map[string]interface{} = connectMethod(connectConfig{
		account:  "DB_SLAVE_ACCOUNT",
		password: "DB_SLAVE_PASSWORD",
		ip:       "DB_SLAVE_IP",
		port:     "DB_SLAVE_PORT",

		db:           "gobike",
		charset:      "utf8mb4",
		location:     "UTC",
		timeout:      "60s",
		maxLifeTime:  900,
		maxOpenConns: 500,
		maxIdleConns: 500,
	})
)

// connectConfig : connect config
type connectConfig struct {
	account  string
	password string
	ip       string
	port     string

	// 寫死
	db           string
	charset      string
	location     string
	timeout      string
	maxLifeTime  int
	maxOpenConns int
	maxIdleConns int
}

// connectMethod : connect method
func connectMethod(conn connectConfig) map[string]interface{} {
	conf := config.EnvForge()
	values := make(map[string]interface{})

	values["Account"] = conf.GetString(conn.account)
	values["Password"] = conf.GetString(conn.password)
	values["IP"] = conf.GetString(conn.ip)
	values["Port"] = conf.GetString(conn.port)
	values["DB"] = conn.db
	values["Charset"] = conn.charset
	values["Location"] = conn.location
	values["Timeout"] = conn.timeout
	values["MaxLifeTime"] = time.Duration(conn.maxLifeTime)
	values["MaxOpenConns"] = conn.maxOpenConns
	values["MaxIdleConns"] = conn.maxIdleConns

	return values
}
