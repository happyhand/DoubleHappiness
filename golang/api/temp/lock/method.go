package lock

import (
	"errors"
	"time"

	"api/temp/ants"
	"api/temp/log"
	"api/temp/redis"

	"github.com/google/uuid"
)

// Lock : 設定 redis 原子鎖
func Lock(key string) (string, error) {
	var (
		result bool   = false
		do     int    = 0
		value  string = uuid.New().String()
	)
	for do < 30 {
		result = redis.SetNX(redis.Cache, 0, key, value, 15*time.Second)
		if result {
			break
		}

		time.Sleep(500 * time.Millisecond)
		do++
	}

	if !result {
		return "", errors.New("set redis lock time out")
	}

	return value, nil
}

// Unlock : 解除 redis 原子鎖
func Unlock(key string, value string) {
	result, err := redis.Get(redis.Cache, 0, key, nil)
	if err != nil {
		return
	}

	code, ok := result.(string)
	if !ok || code != value {
		log.Error("unlock fail", errors.New("redis lock key match error"), map[string]interface{}{
			"key":   key,
			"value": value,
		})
		return
	}

	// 刪除 redis 快取 (非同步)
	ants.Do(nil, func() {
		redis.Delete(redis.Cache, 0, key, false)
	})
}
