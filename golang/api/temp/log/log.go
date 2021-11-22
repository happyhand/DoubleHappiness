package log

import (
	"encoding/json"
	"fmt"
	"os"
	"path/filepath"
	"time"

	"api/temp/config"

	"github.com/sirupsen/logrus"
)

var flag bool = false
var logDir string = config.EnvForge().GetString("LOG_DIR")
var infoLogDir string = fmt.Sprintf("%s/info/", logDir)
var warnLogDir string = fmt.Sprintf("%s/warn/", logDir)
var errorLogDir string = fmt.Sprintf("%s/error/", logDir)
var fatalLogDir string = fmt.Sprintf("%s/fatal/", logDir)

func init() {
	logrus.SetFormatter(&logrus.TextFormatter{
		TimestampFormat: "2006/01/02 15:04:05",
	})
	SetHook(&hook{})
	if flag {
		checkLogDir(filepath.Dir(infoLogDir))
		checkLogDir(filepath.Dir(warnLogDir))
		checkLogDir(filepath.Dir(errorLogDir))
		checkLogDir(filepath.Dir(fatalLogDir))
	}
}

// SetHook :: log set hook
func SetHook(hook logrus.Hook) {
	logrus.AddHook(hook)
}

// Info :: log info
func Info(message string, appendFields map[string]interface{}) {
	if flag {
		file, err := os.OpenFile(logFilePath(infoLogDir), os.O_CREATE|os.O_WRONLY|os.O_APPEND, os.ModePerm)
		if err == nil {
			logrus.StandardLogger().Out = file
			defer file.Close()
		}
	}

	transformFieldJSON(appendFields)
	logrus.WithFields(appendFields).Info(message)
}

// Warn :: log warn
func Warn(message string, err error, appendFields map[string]interface{}) {
	if flag {
		file, err := os.OpenFile(logFilePath(warnLogDir), os.O_CREATE|os.O_WRONLY|os.O_APPEND, os.ModePerm)
		if err == nil {
			logrus.StandardLogger().Out = file
			defer file.Close()
		}
	}

	transformFieldJSON(appendFields)
	logrus.WithFields(appendFields).Warn(message)
}

// Error :: log error
func Error(message string, err error, appendFields map[string]interface{}) {
	if flag {
		file, err := os.OpenFile(logFilePath(errorLogDir), os.O_CREATE|os.O_WRONLY|os.O_APPEND, os.ModePerm)
		if err == nil {
			logrus.StandardLogger().Out = file
			defer file.Close()
		}
	}

	transformFieldJSON(appendFields)
	logrus.WithFields(appendFields).WithError(err).Error(message)
}

// Fatal :: log fatal
func Fatal(message string, err error, appendFields map[string]interface{}) {
	if flag {
		file, err := os.OpenFile(logFilePath(fatalLogDir), os.O_CREATE|os.O_WRONLY|os.O_APPEND, os.ModePerm)
		if err == nil {
			logrus.StandardLogger().Out = file
			defer file.Close()
		}
	}

	transformFieldJSON(appendFields)
	logrus.WithFields(appendFields).WithError(err).Fatal(message)
}

// checkLogDir :: check log dir
func checkLogDir(dir string) {
	_, err := os.Stat(dir)
	if os.IsNotExist(err) {
		_ = os.MkdirAll(dir, os.ModePerm)
	}
}

// logFilePath :: log file path
func logFilePath(dir string) string {
	return fmt.Sprintf("%s%s.log", dir, time.Now().UTC().Format("20060102-15"))
}

// transformFieldJSON :: 轉換附加欄位資料為 json 格式
func transformFieldJSON(appendFields map[string]interface{}) {
	for key, value := range appendFields {
		b, err := json.Marshal(value)
		if err == nil {
			appendFields[key] = string(b)
		}
	}
}
