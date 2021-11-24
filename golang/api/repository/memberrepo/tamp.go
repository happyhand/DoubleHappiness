package memberrepo

import (
	"fmt"

	"api/temp/database"
)

// UpdatePassword : 更新會員密碼
func UpdatePassword(memberID string, password string) error {
	db, err := database.ConnectMaster(database.Master)
	if err != nil {
		return err
	}

	sql := fmt.Sprintf("UPDATE useraccount SET Password = %q WHERE MemberID = %q", password, memberID)
	_, err = db.Exec(sql)
	if err != nil {
		return err
	}

	return nil
}

// EditInfo : 編輯會員資訊
func EditInfo(memberID string, valueMap map[string]interface{}) error {
	db, err := database.ConnectMaster(database.Master)
	if err != nil {
		return err
	}

	var (
		params        []interface{}
		isFirstParams bool = true
	)

	sql := "UPDATE useraccount as ua,userinfo as ui SET"
	for key, value := range valueMap {
		params = append(params, value)
		if isFirstParams {
			sql += fmt.Sprintf(" %s = ?", key)
			isFirstParams = false
			continue
		}

		sql += fmt.Sprintf(", %s = ?", key)
	}

	sql += fmt.Sprintf(" WHERE ua.MemberID = ui.MemberID AND ua.MemberID = %q", memberID)
	_, err = db.Exec(sql, params...)
	if err != nil {
		fmt.Println("sql:", sql)
		return err
	}

	return nil
}
