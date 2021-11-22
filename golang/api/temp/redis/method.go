package redis

import (
	"encoding/json"
	"time"

	"api/temp/ants"
	"api/temp/log"

	"github.com/go-redis/redis"
)

// Set : set redis value with Set method
func Set(config ConnectConfig, db int, key string, value interface{}, expiration time.Duration) {
	client, err := GetClient(config, db)
	if err != nil {
		log.Error("set redis value fail, connect redis error", err, map[string]interface{}{
			"connection": config.Connection,
			"db":         db,
		})
		return
	}

	jsonByte, err := json.Marshal(value)
	if err != nil {
		log.Error("set redis value fail, json marshal error", err, map[string]interface{}{
			"key":   key,
			"value": value,
		})
		return
	}

	_, err = client.Set(key, jsonByte, expiration).Result()
	if err != nil {
		log.Error("set redis value error", err, map[string]interface{}{
			"key":   key,
			"value": value,
		})
	}
}

// SetNX : set redis value with SetNX method
func SetNX(config ConnectConfig, db int, key string, value interface{}, expiration time.Duration) bool {
	client, err := GetClient(config, db)
	if err != nil {
		log.Error("setnx redis value fail, connect redis error", err, map[string]interface{}{
			"connection": config.Connection,
			"db":         db,
		})
		return false
	}

	jsonByte, err := json.Marshal(value)
	if err != nil {
		log.Error("setnx redis value fail, json marshal error", err, map[string]interface{}{
			"key":   key,
			"value": value,
		})
		return false
	}

	result, err := client.SetNX(key, jsonByte, expiration).Result()
	if err != nil {
		log.Error("setnx redis value error", err, map[string]interface{}{
			"key":   key,
			"value": value,
		})
	}

	return result
}

// Get : get redis value with Get method
func Get(config ConnectConfig, db int, key string, otherGetValueFunc func() (value interface{}, expiration time.Duration, err error)) (interface{}, error) {
	// redis 取值
	client, err := GetClient(config, db)
	if err == nil {
		var result string
		var value interface{}
		result, err = client.Get(key).Result()
		if err == nil {
			err = json.Unmarshal([]byte(result), &value)
			if err == nil {
				return value, nil
			}
		}
	}

	// 忽略空值 error
	if err == redis.Nil {
		err = nil
	}

	// 記錄 error
	if err != nil {
		log.Error("get redis value fail", err, map[string]interface{}{
			"key": key,
		})
	}

	// 如果不再執行其他取值方法, 直接回傳 redis 取值結果
	if otherGetValueFunc == nil {
		return nil, err
	}

	// 執行其他取值方法, 並將結果寫回 redis
	value, expiration, err := otherGetValueFunc()
	if err == nil && value != nil && expiration >= 0 {
		ants.Do(nil, func() {
			Set(config, db, key, value, expiration)
		})
	}

	return value, err
}

// Exists : check redis key exsits
func Exists(config ConnectConfig, db int, key string) bool {
	client, err := GetClient(config, db)
	if err != nil {
		log.Error("check redis key exsits fail, connect redis error", err, map[string]interface{}{
			"connection": config.Connection,
			"db":         db,
		})
		return false
	}

	result, err := client.Exists(key).Result()
	if err != nil {
		log.Error("check redis key exsits error", err, map[string]interface{}{
			"key": key,
		})

		return false
	}

	return result == 1
}

// Delete : delete redis value
func Delete(config ConnectConfig, db int, key string, isFuzzy bool) bool {
	client, _ := GetClient(config, db)
	var (
		result      int64
		resultError error
	)
	if isFuzzy {
		// 避免 keys 阻塞線程，故採用 scan 搜尋 key
		var keys []string
		var cursor uint64
		for {
			keys, cursor, resultError = client.Scan(cursor, key, 10).Result()
			if resultError != nil {
				break
			}

			for _, fuzzyKey := range keys {
				result, resultError = client.Del(fuzzyKey).Result()
				if resultError != nil {
					result = 0
					break
				}
			}

			if resultError != nil || cursor == 0 {
				break
			}

			if cursor == 0 {
				break
			}
		}
	} else {
		result, resultError = client.Del(key).Result()
	}

	if resultError != nil {
		log.Error("delete redis value error", resultError, map[string]interface{}{
			"ConnectName": config.Name,
			"db":          db,
			"key":         key,
			"isFuzzy":     isFuzzy,
		})

		return false
	}

	return result == 1
}
