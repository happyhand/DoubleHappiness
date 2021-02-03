package config

import (
	"strings"
	"sync"

	"tool/log"

	"github.com/spf13/viper"
)

var (
	envInstance *viper.Viper
	envOnce     sync.Once
)

// EnvForge : 取得實例
func EnvForge() *viper.Viper {
	envOnce.Do(func() {
		envInstance = viper.New()
		envInstance.SetConfigName("env")
		envInstance.SetConfigType("yml")
		envInstance.AddConfigPath(".")
		envInstance.SetEnvKeyReplacer(strings.NewReplacer(".", "_"))
		envInstance.AutomaticEnv()

		err := envInstance.ReadInConfig()
		if err != nil {
			log.Error("read config error", err, nil)
		}

		if err := envInstance.ReadInConfig(); err != nil {
			log.Error("Could not find env configuration file", err, nil)
		}
	})

	return envInstance
}
