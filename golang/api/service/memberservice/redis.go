package memberservice

import (
	"api/core/config"
	"api/temp/ants"
	"api/temp/redis"
	"fmt"
)

// ClearRedis : 清除會員相關 redis
func ClearRedis(memberID string) {
	ants.Do(nil, func() {
		redis.Delete(redis.Cache, config.MemberDB, fmt.Sprintf("*-U-%s*", memberID), true)
	})
}
