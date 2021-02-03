package log

import "github.com/sirupsen/logrus"

func init() {
	logrus.SetFormatter(&logrus.TextFormatter{
		DisableSorting: true,
	})
	SetHook(&hook{})
}

// SetHook :: log set hook
func SetHook(hook logrus.Hook) {
	logrus.AddHook(hook)
}

// Info :: log info
func Info(message string, appendFields map[string]interface{}) {
	logrus.WithFields(appendFields).Info(message)
}

// Warn :: log warn
func Warn(message string, err error, appendFields map[string]interface{}) {
	logrus.WithFields(appendFields).WithError(err).Warn(message)
}

// Error :: log error
func Error(message string, err error, appendFields map[string]interface{}) {
	logrus.WithFields(appendFields).WithError(err).Error(message)
}

// Fatal :: log fatal
func Fatal(message string, err error, appendFields map[string]interface{}) {
	logrus.WithFields(appendFields).WithError(err).Fatal(message)
}
