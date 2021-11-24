package serverrepo

import (
	"encoding/json"
	"errors"

	"api/repository/memberrepo"
	"api/repository/serverrepo/usermodel"
	"api/temp/database"
)

// DoAction : 執行後端指令
func BackupDoAction(commandID int, commandType int, data interface{}, receive interface{}) error {
	var response []byte
	var err error
	switch commandType {
	case UserCommandType:
		switch commandID {
		case usermodel.UserLoginCommandID:
			response, err = userLogin(data.(usermodel.LoginRequest))
		case usermodel.UpdateUserInfoCommandID:
			response, err = editInfo(data.(usermodel.EditInfoRequest))
		default:
			err = errors.New("illegal server command id")
		}
	case RideCommandType:

	case TeamCommandType:

	default:
		err = errors.New("illegal server command type")
	}

	if err != nil {
		return err
	}

	return json.Unmarshal([]byte(response), receive)
}

func userLogin(request usermodel.LoginRequest) ([]byte, error) {
	res := usermodel.LoginResponse{}
	daos, err := memberrepo.Search(map[string]interface{}{
		"Email": request.Email,
	}, true, false, nil)

	if err != nil {
		return nil, err
	}

	if len(daos) == 0 {
		res.Result = usermodel.UserLoginResultEmailError
		return json.Marshal(res)
	}

	dao := daos[0]
	res.MemberID = dao.MemberID
	res.Result = usermodel.UserLoginResultSuccess
	return json.Marshal(res)
}

func editInfo(request usermodel.EditInfoRequest) ([]byte, error) {
	var sqlTag string = "joindb"
	var sqlDao *memberrepo.MemberDao = &memberrepo.MemberDao{}
	_, _, sqlColumnMap := database.SQLColumn(sqlDao, sqlTag)
	valueMap := map[string]interface{}{}
	if request.UpdateData.Avatar != nil {

		valueMap[sqlColumnMap["Avatar"]] = request.UpdateData.Avatar
	}

	if request.UpdateData.Birthday != nil {
		valueMap[sqlColumnMap["Birthday"]] = request.UpdateData.Birthday
	}

	if request.UpdateData.BodyHeight != nil {
		valueMap[sqlColumnMap["BodyHeight"]] = request.UpdateData.BodyHeight
	}

	if request.UpdateData.BodyWeight != nil {
		valueMap[sqlColumnMap["BodyWeight"]] = request.UpdateData.BodyWeight
	}

	if request.UpdateData.FrontCover != nil {
		valueMap[sqlColumnMap["FrontCover"]] = request.UpdateData.FrontCover
	}

	if request.UpdateData.Gender != nil {
		valueMap[sqlColumnMap["Gender"]] = request.UpdateData.Gender
	}

	if request.UpdateData.Mobile != nil {
		valueMap[sqlColumnMap["Mobile"]] = request.UpdateData.Mobile
	}

	if request.UpdateData.NickName != nil {
		valueMap[sqlColumnMap["Nickname"]] = request.UpdateData.NickName
	}

	if request.UpdateData.Photo != nil {
		valueMap[sqlColumnMap["Photo"]] = request.UpdateData.Photo
	}

	err := memberrepo.EditInfo(request.MemberID, valueMap)
	if err != nil {
		return nil, err
	}

	return json.Marshal(usermodel.EditInfoResponse{
		Result: usermodel.UpdateUserInfoResultSuccess,
	})
}
