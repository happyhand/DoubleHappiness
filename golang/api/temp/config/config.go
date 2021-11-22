package config

import (
	"fmt"
	"strings"
	"sync"

	"github.com/sirupsen/logrus"
	"github.com/spf13/viper"
)

var (
	// envInstance : env 實例
	envInstance *viper.Viper
	// envOnce : env 同步鎖
	envOnce sync.Once
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
			logrus.Error(fmt.Errorf("read env error:%s", err.Error()))
		}

		if err := envInstance.ReadInConfig(); err != nil {
			logrus.Error(fmt.Errorf("could not find env configuration file:%s", err.Error()))
		}
	})
	return envInstance
}
