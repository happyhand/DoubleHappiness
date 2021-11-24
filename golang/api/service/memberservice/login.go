package memberservice

import (
	"errors"
	"fmt"
	"time"

	"api/core/config"
	"api/core/customerror"
	"api/repository/memberrepo"
	"api/repository/serverrepo"
	"api/repository/serverrepo/usermodel"

	"api/temp/ants"
	"api/temp/log"
	"api/temp/redis"
)

// Login : 會員登入
func Login(params LoginParams) (*MemberDto, *customerror.Error) {
	// 發送【使用者登入】指令至後端
	request := usermodel.LoginRequest{
		Avatar:      params.Avatar,
		Email:       params.Email,
		LoginSource: params.LoginSource,
		NickName:    params.NickName,
		Token:       params.Token,
	}

	var response usermodel.LoginResponse
	err := serverrepo.DoAction(usermodel.UserLoginCommandID, serverrepo.UserCommandType, request, &response)
	if err != nil {
		return nil, customerror.New(customerror.SystemError, err.Error())
	}

	switch response.Result {
	case usermodel.UserLoginResultSuccess:
		dao, err := memberrepo.Get(response.MemberID)
		if err != nil {
			return nil, customerror.New(customerror.SystemError, err.Error())
		}

		if dao == nil {
			err := errors.New("not found member data")
			log.Error("login fail", err, map[string]interface{}{
				"MemberID": response.MemberID,
			})
			return nil, customerror.New(customerror.DataNotExistError, err.Error())
		}

		dto := TransformMemberDto(*dao, true)
		return &dto, nil
	case usermodel.UserLoginResultFail:
		return nil, customerror.New(customerror.LoginFail, "server refuse login")
	case usermodel.UserLoginResultEmailError, usermodel.UserLoginResultPasswordError:
		return nil, customerror.New(customerror.EmailOrPasswordNotMatch, "email or password not match")
	default:
		return nil, customerror.New(customerror.SystemError, "illegal server response result")
	}
}

// UpdateLastLoginDate : 更新會員最新登入時間
func UpdateLastLoginDate(memberID string) {
	date := time.Now().UTC().Unix()
	redisKey := fmt.Sprintf(config.MemberLastLoginRedisKey, memberID)
	ants.Do(nil, func() {
		redis.Set(redis.Cache, config.MemberDB, redisKey, date, config.KeepOnlineTimeout)
	})
}
