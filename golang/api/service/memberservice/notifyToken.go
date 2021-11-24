package memberservice

import (
	"api/core/customerror"
	"api/repository/serverrepo"
	"api/repository/serverrepo/usermodel"
)

// UpdateNotifyToken : 會員更新 notify token
func UpdateNotifyToken(params UpdateNotifyTokenParams) *customerror.Error {
	// 發送【會員更新推播 Token】指令至後端
	request := usermodel.UpdateNotifyTokenRequest{
		MemberID:    params.MemberID,
		NotifyToken: params.NotifyToken,
	}

	var response usermodel.UpdateNotifyTokenResponse
	err := serverrepo.DoAction(usermodel.UpdateNotifyTokenCommandID, serverrepo.UserCommandType, request, &response)
	if err != nil {
		return customerror.New(customerror.SystemError, err.Error())
	}

	switch response.Result {
	case usermodel.UpdateNotifyTokenResultSuccess:
		return nil
	case usermodel.UpdateNotifyTokenResultFail:
		return customerror.New(customerror.UpdateDataError, "server refuse update notify token")
	default:
		return customerror.New(customerror.SystemError, "illegal server response result")
	}
}
