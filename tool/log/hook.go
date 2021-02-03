package log

import (
	"fmt"
	"os"
	"runtime"
	"strings"

	"github.com/sirupsen/logrus"
)

// hook :: logrus 自訂外掛
type hook struct {
}

// wrapper :: 格式封裝
type wrapper struct {
	logrusFormatter logrus.Formatter
}

// formatter :: 自訂格式
var formatter logrus.Formatter

// Levels :: logrus Hook interface Levels
func (hook *hook) Levels() []logrus.Level {
	return logrus.AllLevels
}

// Fire :: logrus Hook interface Fire
func (hook *hook) Fire(entry *logrus.Entry) error {
	if formatter != entry.Logger.Formatter {
		formatter = &wrapper{
			logrusFormatter: entry.Logger.Formatter,
		}
	}

	entry.Logger.Formatter = formatter
	return nil
}

// Format :: logrus Formatter interface Fire
func (w *wrapper) Format(entry *logrus.Entry) ([]byte, error) {
	modified := entry.WithField("path", logPath())
	modified.Level = entry.Level
	modified.Message = entry.Message
	return w.logrusFormatter.Format(modified)
}

// logPath :: 取得紀錄路徑
func logPath() string {
	function, file, line, _ := runtime.Caller(7)
	functionName := runtime.FuncForPC(function).Name()
	dir, err := os.Getwd()
	if err == nil {
		file = strings.ReplaceAll(file, dir, "")
	}

	params := strings.Split(functionName, ".")
	return fmt.Sprintf("%s:%d - %s()", file, line, params[1])
}
