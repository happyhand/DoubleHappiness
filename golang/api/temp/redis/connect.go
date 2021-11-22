package redis

import (
	"fmt"
	"sync"

	"api/temp/log"

	"github.com/go-redis/redis"
)

var (
	// pool : Redis 連線物件池
	pool map[string]*redis.Client

	// mu : 同步鎖
	mu *sync.Mutex
)

func init() {
	pool = make(map[string]*redis.Client)
	mu = &sync.Mutex{}
}

// GetClient : get redis client
func GetClient(config ConnectConfig, db int) (*redis.Client, error) {
	mu.Lock()
	defer mu.Unlock()

	poolName := fmt.Sprintf("%s_%d", config.Name, db)
	if redisConn, err := pool[poolName]; err {
		return redisConn, nil
	}

	var err error
	pool[poolName], err = connect(config, db)
	return pool[poolName], err
}

// connect : 建立Redis連線
func connect(config ConnectConfig, db int) (*redis.Client, error) {
	client := redis.NewClient(&redis.Options{
		Addr:         config.Connection,
		Password:     "",
		DB:           db,
		DialTimeout:  config.DialTimeout,
		ReadTimeout:  config.ReadTimeout,
		WriteTimeout: config.ReadTimeout,
		PoolSize:     config.PoolSize,
		PoolTimeout:  config.PoolTimeout,
	})

	_, err := client.Ping().Result()
	if err != nil {
		log.Error("connection to redis fail", err, map[string]interface{}{
			"config": config,
		})
		return nil, err
	}

	return client, nil
}
