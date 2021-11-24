package rideservice

import (
	"api/core/customerror"
	"api/repository/riderepo"
)

// GetDistance : 取得騎乘距離資訊
func GetDistance(memberID string) (*RideDistanceDto, *customerror.Error) {
	dao, err := riderepo.GetDistance(memberID)
	if err != nil {
		return nil, customerror.New(customerror.SystemError, err.Error())
	}

	if dao == nil {
		return nil, nil
	}

	dto := TransformRideDistanceDto(*dao)
	return &dto, nil
}

// TransformRideDistanceDto : 轉換騎乘距離資訊
func TransformRideDistanceDto(dao riderepo.RideDistanceDao) RideDistanceDto {
	return RideDistanceDto{
		MemberID:      dao.MemberID,
		TotalDistance: dao.TotalDistance,
		WeekDistance:  dao.WeekDistance,
	}
}
