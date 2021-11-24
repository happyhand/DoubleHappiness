package verificationcodeservice

import (
	"fmt"

	"api/core/config"
	"api/core/customerror"
	"api/repository/memberrepo"

	"api/temp/ants"
	"api/temp/redis"

	"github.com/google/uuid"
)

// Generate : 生成驗證碼
func Generate(params GenerateParams) (string, *customerror.Error) {
	exist := memberrepo.ExistEmail(params.Email)
	if !exist {
		return "", customerror.New(customerror.IllegalEmail, "illegal email")
	}

	code := uuid.NewString()[:8]
	redisKey := fmt.Sprintf(config.VerificationCodeRedisKey, params.Type, params.Email)
	ok := redis.SetNX(redis.Cache, config.CommonDB, redisKey, code, config.VerificationCodeTimeout)
	if !ok {
		return "", &customerror.Error{
			Code:    customerror.VerificationCodeGenerateFail,
			Message: "verification code already exists",
		}
	}

	return code, nil
}

// Validate : 驗證驗證碼
func Validate(params ValidateParams) (bool, *customerror.Error) {
	redisKey := fmt.Sprintf(config.VerificationCodeRedisKey, params.Type, params.Email)
	value, err := redis.Get(redis.Cache, config.CommonDB, redisKey, nil)
	if err != nil {
		return false, &customerror.Error{
			Code:    customerror.SystemError,
			Message: err.Error(),
		}
	}

	if value == nil {
		return false, nil
	}

	return value == params.Code, nil
}

// Delete : 刪除驗證碼
func Delete(params DeleteParams) {
	redisKey := fmt.Sprintf(config.VerificationCodeRedisKey, params.Type, params.Email)
	ants.Do(nil, func() {
		redis.Delete(redis.Cache, config.CommonDB, redisKey, false)
	})
}
