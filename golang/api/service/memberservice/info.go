package memberservice

import (
	"api/core/config"
	"api/core/customerror"
	"api/repository/memberrepo"
	"api/repository/serverrepo"
	"api/repository/serverrepo/usermodel"
	"api/temp/log"
	"api/temp/redis"
	"errors"
	"fmt"
)

// Search : 搜尋會員
func Search(params SearchParams) ([]MemberDto, *customerror.Error) {
	// 目前只開放 Email 跟 Nickname 提供搜尋
	keyMap := map[string]interface{}{
		"Email":    params.Key,
		"Nickname": params.Key,
	}

	daos, err := memberrepo.Search(keyMap, false, params.IsFuzzy, []string{params.MemberID})
	if err != nil {
		return nil, customerror.New(customerror.SystemError, err.Error())
	}

	var dtos []MemberDto
	for _, dao := range daos {
		if !params.CheckSearchStatus || dao.SearchStatus == 1 {
			var dto MemberDto = TransformMemberDto(dao, true)
			dtos = append(dtos, dto)
		}
	}

	return dtos, nil
}

// GetInfo : 取得會員資訊
func GetInfo(params GetInfoParams) (*MemberDto, *customerror.Error) {
	dao, err := memberrepo.Get(params.TargetID)
	if err != nil {
		return nil, customerror.New(customerror.SystemError, err.Error())
	}

	if dao == nil {
		return nil, customerror.New(customerror.DataNotExistError, "not found member data")
	}

	if params.TargetID != params.SearchID && dao.SearchStatus != 1 {
		return nil, customerror.New(customerror.PrivateDataError, "member data is private")
	}

	dto := TransformMemberDto(*dao, true)
	return &dto, nil
}

// EditInfo : 會員編輯資訊
func EditInfo(params EditInfoParams) (*MemberDto, *MemberDto, *customerror.Error) {
	// 抓取舊資料
	dao, err := memberrepo.Get(params.MemberID)
	if err != nil {
		return nil, nil, customerror.New(customerror.SystemError, err.Error())
	}

	if dao == nil {
		err := errors.New("not found member data")
		log.Error("edit info fail", err, map[string]interface{}{
			"MemberID": params.MemberID,
		})
		return nil, nil, customerror.New(customerror.DataNotExistError, err.Error())
	}

	oldDto := TransformMemberDto(*dao, true)

	// 發送【更新使用者資訊】指令至後端
	request := usermodel.EditInfoRequest{
		MemberID: params.MemberID,
		UpdateData: struct {
			Avatar     *string
			Birthday   *string
			BodyHeight *float64
			BodyWeight *float64
			County     *int
			FrontCover *string
			Gender     *int
			Mobile     *string
			NickName   *string
			Photo      *string
		}{
			Avatar:     params.Avatar,
			BodyHeight: params.BodyHeight,
			BodyWeight: params.BodyWeight,
			County:     params.County,
			FrontCover: params.FrontCover,
			Gender:     params.Gender,
			NickName:   params.Nickname,
			Photo:      params.Photo,
		},
	}

	if params.Birthday != nil {
		date := params.Birthday.Format("2006-01-02T15:04:05")
		request.UpdateData.Birthday = &date
	}

	var response usermodel.EditInfoResponse
	err = serverrepo.DoAction(usermodel.UpdateUserInfoCommandID, serverrepo.UserCommandType, request, &response)
	if err != nil {
		return nil, nil, customerror.New(customerror.SystemError, err.Error())
	}

	switch response.Result {
	case usermodel.UpdateUserInfoResultSuccess:
		dao, err = memberrepo.Get(params.MemberID)
		if err != nil {
			return nil, nil, customerror.New(customerror.SystemError, err.Error())
		}

		if dao == nil {
			err := errors.New("not found member data")
			log.Error("edit info fail", err, map[string]interface{}{
				"MemberID": params.MemberID,
			})
			return nil, nil, customerror.New(customerror.DataNotExistError, err.Error())
		}

		dto := TransformMemberDto(*dao, true)
		return &oldDto, &dto, nil
	case usermodel.UpdateUserInfoResultFail:
		return nil, nil, customerror.New(customerror.UpdateDataError, "server refuse update user info")
	default:
		return nil, nil, customerror.New(customerror.SystemError, "illegal server response result")
	}
}

// MobileBind : 手機綁定
func MobileBind(params MobileBindParams) *customerror.Error {
	dao, err := memberrepo.Get(params.MemberID)
	if err != nil {
		return customerror.New(customerror.SystemError, err.Error())
	}

	if dao == nil {
		err := errors.New("not found member data")
		log.Error("mobile bind fail", err, map[string]interface{}{
			"MemberID": params.MemberID,
		})
		return customerror.New(customerror.DataNotExistError, err.Error())
	}

	if dao.Mobile != "" {
		return customerror.New(customerror.MobileBind, "already bound mobile")
	}

	if memberrepo.ExistMobile(params.Mobile) {
		return customerror.New(customerror.DataRepeatFail, "mobile has been bound")
	}

	// 發送【更新使用者資訊】指令至後端
	request := usermodel.EditInfoRequest{
		MemberID: params.MemberID,
		UpdateData: struct {
			Avatar     *string
			Birthday   *string
			BodyHeight *float64
			BodyWeight *float64
			County     *int
			FrontCover *string
			Gender     *int
			Mobile     *string
			NickName   *string
			Photo      *string
		}{
			Mobile: &params.Mobile,
		},
	}

	var response usermodel.EditInfoResponse
	err = serverrepo.DoAction(usermodel.UpdateUserInfoCommandID, serverrepo.UserCommandType, request, &response)
	if err != nil {
		return customerror.New(customerror.SystemError, err.Error())
	}

	switch response.Result {
	case usermodel.UpdateUserInfoResultSuccess:
		return nil
	case usermodel.UpdateUserInfoResultFail:
		return customerror.New(customerror.UpdateDataError, "server refuse update user info")
	default:
		return customerror.New(customerror.SystemError, "illegal server response result")
	}
}

// TransformMemberDto : 轉換會員資訊
func TransformMemberDto(dao memberrepo.MemberDao, isShadeOnlineStatus bool) MemberDto {
	var (
		hasTeam    int = 0
		onlineType int = 0
	)

	if dao.TeamList != "" {
		hasTeam = 1
	}

	if !isShadeOnlineStatus {
		onlineTypeRedisKey := fmt.Sprintf(config.MemberLastLoginRedisKey, dao.MemberID)
		exist := redis.Exists(redis.Cache, config.MemberDB, onlineTypeRedisKey)
		if exist {
			onlineType = 1
		} else {
			onlineType = -1
		}
	}

	return MemberDto{
		Avatar:     dao.Avatar,
		Birthday:   dao.Birthday,
		BodyHeight: dao.BodyHeight,
		BodyWeight: dao.BodyWeight,
		County:     dao.County,
		Email:      dao.Email,
		FrontCover: dao.FrontCover,
		Gender:     dao.Gender,
		HasTeam:    hasTeam,
		MemberID:   dao.MemberID,
		Mobile:     dao.Mobile,
		Nickname:   dao.Nickname,
		OnlineType: onlineType,
		Photo:      dao.Photo,
	}
}
