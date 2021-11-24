package rideservice

import (
	"api/core/customerror"
	"api/repository/riderepo"
	"api/repository/serverrepo"
	"api/repository/serverrepo/ridemodel"
	"encoding/json"
	"time"
)

// AddRecord : 新增騎乘紀錄
func AddRecord(params AddRecordParams) *customerror.Error {
	// 發送【建立騎乘紀錄】指令至後端
	shareContent := [][]string{}
	for index, content := range params.ShareContent {
		if index < len(params.ShareImage) {
			shareContent = append(shareContent, []string{content, params.ShareImage[index]})
		} else {
			shareContent = append(shareContent, []string{content, ""})
		}
	}

	shareContentJSON, err := json.Marshal(shareContent)
	if err != nil {
		return customerror.New(customerror.SystemError, err.Error())
	}

	if params.Title == "" {
		params.Title = time.Now().UTC().Format("2006/01/02")
	}

	request := ridemodel.AddRecordRequest{
		Altitude:     params.Altitude,
		County:       params.County,
		Distance:     params.Distance,
		Level:        params.Level,
		MemberID:     params.MemberID,
		Photo:        params.Photo,
		Route:        params.Route,
		ShareContent: string(shareContentJSON),
		SharedType:   params.SharedType,
		Time:         params.Time,
		Title:        params.Title,
	}

	var response ridemodel.AddRecordResponse
	err = serverrepo.DoAction(ridemodel.CreateRideRecordCommandID, serverrepo.RideCommandType, request, &response)
	if err != nil {
		return customerror.New(customerror.SystemError, err.Error())
	}

	switch response.Result {
	case ridemodel.CreateRideRecordResultSuccess:
		return nil
	case ridemodel.CreateRideRecordResultFail:
		return customerror.New(customerror.InsertDataError, "server refuse add ride record")
	default:
		return customerror.New(customerror.SystemError, "illegal server response result")
	}
}

// GetRecord : 取得騎乘紀錄列表
func GetRecord(memberID string) ([]RideRecordDto, *customerror.Error) {
	daos, err := riderepo.GetRecord(memberID)
	if err != nil {
		return nil, customerror.New(customerror.SystemError, err.Error())
	}

	var dtos []RideRecordDto
	for _, dao := range daos {
		dtos = append(dtos, TransformRideRecordDto(dao))
	}

	return dtos, nil
}

// GetAppointRecord : 取得指定騎乘紀錄列表
func GetAppointRecord(rideID string) (*RideRecordDto, *customerror.Error) {
	dao, err := riderepo.GetAppointRecord(rideID)
	if err != nil {
		return nil, customerror.New(customerror.SystemError, err.Error())
	}

	if dao == nil {
		return nil, customerror.New(customerror.DataNotExistError, "not found ride record data")
	}

	dto := TransformRideRecordDto(*dao)
	return &dto, nil
}

// TransformRideRecordDto : 轉換騎乘紀錄資訊
func TransformRideRecordDto(dao riderepo.RideRecordDao) RideRecordDto {
	return RideRecordDto{
		Altitude:     dao.Altitude,
		County:       dao.County,
		CreateDate:   dao.CreateDate,
		Distance:     dao.Distance,
		Level:        dao.Level,
		MemberID:     dao.MemberID,
		Photo:        dao.Photo,
		RideID:       dao.RideID,
		Route:        dao.Route,
		ShareContent: dao.ShareContent,
		SharedType:   dao.SharedType,
		Time:         dao.Time,
		Title:        dao.Title,
	}
}
