package riderepo

import (
	"api/temp/database"
	"api/temp/log"
	"database/sql"
	"fmt"
)

// GetDistance : 取得會員騎乘距離資料
func GetDistance(memberID string) (*RideDistanceDao, error) {
	db, err := database.ConnectSlave(database.Slave)
	if err != nil {
		log.Error("get ride distance data fail, connect db error", err, nil)
		return nil, err
	}

	var sqlTag string = "joindb"
	var sqlDao *RideDistanceDao = &RideDistanceDao{}
	_, sqlColumn, _ := database.SQLColumn(sqlDao, sqlTag)
	query := fmt.Sprintf("SELECT %s FROM ridedata as rd INNER JOIN weekridedata as wrd ON rd.MemberID=wrd.MemberID WHERE rd.MemberID = %q", sqlColumn, memberID)
	err = db.QueryRowx(query).Scan(database.SQLColumnValue(sqlDao, sqlTag)...)
	if err != nil {
		if err == sql.ErrNoRows {
			return nil, nil
		}

		log.Error("get ride distance data fail, row scan error", err, map[string]interface{}{
			"sql": query,
		})
		return nil, err
	}

	return sqlDao, nil
}

// GetRecord : 取得會員騎乘紀錄列表
func GetRecord(memberID string) ([]RideRecordDao, error) {
	db, err := database.ConnectSlave(database.Slave)
	if err != nil {
		log.Error("get multiple ride record data fail, connect db error", err, nil)
		return nil, err
	}

	var sqlTag string = "db"
	var daos []RideRecordDao
	_, sqlColumn, _ := database.SQLColumn(&RideRecordDao{}, sqlTag)
	query := fmt.Sprintf("SELECT %s FROM riderecord WHERE MemberID = %q", sqlColumn, memberID)
	err = db.Select(&daos, query)
	if err != nil {
		log.Error("get multiple ride record data fail, db query error", err, map[string]interface{}{
			"sql": query,
		})
		return nil, err
	}

	return daos, nil
}

// GetAppointRecord : 取得指定騎乘紀錄
func GetAppointRecord(rideID string) (*RideRecordDao, error) {
	db, err := database.ConnectSlave(database.Slave)
	if err != nil {
		log.Error("get ride record data fail, connect db error", err, nil)
		return nil, err
	}

	var sqlTag string = "db"
	var dao *RideRecordDao = &RideRecordDao{}
	_, sqlColumn, _ := database.SQLColumn(dao, sqlTag)
	query := fmt.Sprintf("SELECT %s FROM riderecord WHERE RideID = %q", sqlColumn, rideID)
	err = db.QueryRowx(query).StructScan(dao)
	if err != nil {
		if err == sql.ErrNoRows {
			return nil, nil
		}

		log.Error("get ride record data fail, db query error", err, map[string]interface{}{
			"sql": query,
		})
		return nil, err
	}

	return dao, nil
}
