package redis

import (
	"time"

	"tool/config"
)

var (
	// BaseConfig : redis base config
	BaseConfig map[string]interface{} = connectMethod(connectConfig{
		Name:       "redis",
		Connection: "REDIS",
		DB:         0,

		DialTimeout: 100,
		ReadTimeout: 100,
		PoolSize:    2000,
		PoolTimeout: 3000,
		TTL:         7200,
	})
)

// connectConfig : redis setting
type connectConfig struct {
	Name       string
	Connection string
	DB         int

	DialTimeout int
	ReadTimeout int
	PoolSize    int
	PoolTimeout int
	TTL         int
}

// connectMethod : redis connect method
func connectMethod(conn connectConfig) map[string]interface{} {
	conf := config.EnvForge()

	values := make(map[string]interface{})

	values["Name"] = conn.Name
	values["DB"] = conn.DB
	values["Connection"] = conf.GetString(conn.Connection)
	values["DialTimeout"] = time.Duration(conn.DialTimeout) * time.Second
	values["ReadTimeout"] = time.Duration(conn.ReadTimeout) * time.Second
	values["PoolSize"] = conn.PoolSize
	values["PoolTimeout"] = time.Duration(conn.PoolTimeout) * time.Second
	values["TTL"] = time.Duration(conn.TTL)

	return values
}
