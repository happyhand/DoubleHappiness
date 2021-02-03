package redis

import (
	"sync"
	"time"

	"tool/log"

	"github.com/go-redis/redis"
)

var (
	// pool : Redis 連線物件
	pool map[string]*redis.Client

	// mu : 同步鎖
	mu *sync.Mutex
)

func init() {
	pool = make(map[string]*redis.Client)
	mu = &sync.Mutex{}
}

// GetClient : 取得 redis client
func GetClient(conn map[string]interface{}) (*redis.Client, error) {

	mu.Lock()
	defer mu.Unlock()

	if redisConn, exist := pool[conn["Name"].(string)]; exist {
		return redisConn, nil
	}

	var err error
	pool[conn["Name"].(string)], err = connect(conn)
	return pool[conn["Name"].(string)], err
}

// connect : 建立Redis連線
func connect(values map[string]interface{}) (*redis.Client, error) {
	client := redis.NewClient(&redis.Options{
		Addr:         values["Connection"].(string),
		Password:     "",
		DB:           values["DB"].(int),
		DialTimeout:  values["DialTimeout"].(time.Duration),
		ReadTimeout:  values["ReadTimeout"].(time.Duration),
		WriteTimeout: values["ReadTimeout"].(time.Duration),
		PoolSize:     values["PoolSize"].(int),
		PoolTimeout:  values["PoolTimeout"].(time.Duration),
	})

	_, err := client.Ping().Result()
	if err != nil {
		log.Error("redis connect err", err, nil)
		return nil, err
	}

	return client, nil
}
