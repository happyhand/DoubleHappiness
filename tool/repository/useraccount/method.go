package useraccount

import (
	"fmt"

	"tool/database"
	"tool/log"
)

// GetList : 取得使用者列表
func GetList() ([]User, error) {
	db, err := database.ConnectionSlave(database.SlaveConfig)
	if err != nil {
		log.Error("db connect error", err, nil)
		return nil, err
	}

	sql := fmt.Sprintf("SELECT * FROM %s", TableName)
	rows, err := db.Query(sql)
	if err != nil {
		log.Error("db query error", err, nil)
		return nil, err
	}

	var users []User
	for rows.Next() {
		var result User
		err := rows.Scan(
			&result.MemberID,
			&result.Email,
			&result.Password,
			&result.FBToken,
			&result.GoogleToken,
			&result.NotifyToken,
			&result.RegisterSource,
			&result.RegisterDate,
		)

		if err != nil {
			return nil, err
		}

		users = append(users, result)
	}

	return users, nil
}
