package memberrepo

import (
	"database/sql"
	"fmt"
	"strings"

	"api/temp/database"
	"api/temp/log"
)

// Search : 條件式搜尋會員資料
func Search(keyMap map[string]interface{}, isExactMatch bool, isFuzzy bool, ignoreMemberID []string) ([]MemberDao, error) {
	db, err := database.ConnectSlave(database.Slave)
	if err != nil {
		log.Error("get member data fail, connect db error", err, nil)
		return nil, err
	}

	var (
		params       []interface{}
		isFirstKey   bool   = true
		matchStr     string = "AND"
		conditionStr string = "= ?"
		sqlTag       string = "joindb"
	)
	if !isExactMatch {
		matchStr = "OR"
	}

	if isFuzzy {
		conditionStr = "LIKE CONCAT('%%', ?, '%%')"
	}

	_, sqlColumn, sqlColumnMap := database.SQLColumn(&MemberDao{}, sqlTag)
	query := fmt.Sprintf("SELECT %s FROM useraccount as ua INNER JOIN userinfo as ui ON ua.MemberID=ui.MemberID WHERE", sqlColumn)
	for key, value := range keyMap {
		params = append(params, value)
		column := sqlColumnMap[key]
		if isFirstKey {
			isFirstKey = false
			query += fmt.Sprintf(" %s %s", column, conditionStr)
		} else {
			query += fmt.Sprintf(" %s %s %s", matchStr, column, conditionStr)
		}
	}

	if len(ignoreMemberID) > 0 {
		query += fmt.Sprintf(" AND ua.MemberID NOT IN (%q)", strings.Join(ignoreMemberID, ",")) // 排除忽略對象
	}

	query += " GROUP BY ua.MemberID" // 過濾重複資料
	rows, err := db.Queryx(query, params...)
	if err != nil {
		log.Error("get member data fail, db query error", err, map[string]interface{}{
			"sql":    query,
			"params": params,
		})
		return nil, err
	}

	var daos []MemberDao
	for rows.Next() {
		var dao MemberDao
		err := rows.Scan(database.SQLColumnValue(&dao, sqlTag)...)
		if err != nil {
			log.Error("get member data fail, row scan error", err, map[string]interface{}{
				"sql":    query,
				"params": params,
			})
			return nil, err
		}

		dao.SearchStatus = 1 // TODO 待開放其功能設定
		daos = append(daos, dao)
	}

	return daos, nil
}

// Get : 取得指定會員資料
func Get(memberID string) (*MemberDao, error) {
	db, err := database.ConnectSlave(database.Slave)
	if err != nil {
		log.Error("get member data fail, connect db error", err, nil)
		return nil, err
	}

	var dao *MemberDao = &MemberDao{}
	var sqlTag string = "joindb"
	_, sqlColumn, _ := database.SQLColumn(dao, sqlTag)
	query := fmt.Sprintf("SELECT %s FROM useraccount as ua INNER JOIN userinfo as ui ON ua.MemberID=ui.MemberID WHERE ua.MemberID = %q", sqlColumn, memberID)
	err = db.QueryRowx(query).Scan(database.SQLColumnValue(dao, sqlTag)...)
	if err != nil {
		if err == sql.ErrNoRows {
			return nil, nil
		}

		log.Error("get member data fail, row scan error", err, map[string]interface{}{
			"sql": query,
		})
		return nil, err
	}

	dao.SearchStatus = 1 // TODO 待開放其功能設定
	return dao, nil
}

// GetMultiple : 取得多筆會員資料
func GetMultiple(memberIDs []string, ignoreMemberID []string) ([]MemberDao, error) {
	db, err := database.ConnectSlave(database.Slave)
	if err != nil {
		log.Error("get multiple member data fail, connect db error", err, nil)
		return nil, err
	}

	var sqlTag string = "joindb"
	_, sqlColumn, _ := database.SQLColumn(&MemberDao{}, sqlTag)
	query := fmt.Sprintf("SELECT %s FROM useraccount as ua INNER JOIN userinfo as ui ON ua.MemberID=ui.MemberID WHERE", sqlColumn)
	query += fmt.Sprintf(" ua.MemberID IN (%q)", strings.Join(memberIDs, ",")) // 排除忽略對象
	if len(ignoreMemberID) > 0 {
		query += fmt.Sprintf(" AND ua.MemberID NOT IN (%q)", strings.Join(ignoreMemberID, ",")) // 排除忽略對象
	}

	query += " GROUP BY ua.MemberID" // 過濾重複資料
	rows, err := db.Queryx(query)
	if err != nil {
		log.Error("get multiple member data fail, db query error", err, map[string]interface{}{
			"sql": query,
		})
		return nil, err
	}

	var daos []MemberDao
	for rows.Next() {
		var dao MemberDao
		err := rows.Scan(database.SQLColumnValue(&dao, sqlTag)...)
		if err != nil {
			log.Error("get multiple member data fail, row scan error", err, map[string]interface{}{
				"sql": query,
			})
			return nil, err
		}

		dao.SearchStatus = 1 // TODO 待開放其功能設定
		daos = append(daos, dao)
	}

	return daos, nil
}

// ExistEmail : 檢查信箱是否存在
func ExistEmail(email string) bool {
	db, err := database.ConnectSlave(database.Slave)
	if err != nil {
		log.Error("check email exist fail, connect db error", err, nil)
		return false
	}

	var count int
	query := fmt.Sprintf("SELECT COUNT(*) FROM useraccount WHERE Email = %q", email)
	err = db.QueryRowx(query).Scan(&count)
	if err != nil {
		log.Error("check email exist fail, db query error", err, map[string]interface{}{
			"sql": query,
		})
		return false
	}

	return count > 0
}

// ExistMobile : 檢查手機是否存在
func ExistMobile(mobile string) bool {
	db, err := database.ConnectSlave(database.Slave)
	if err != nil {
		log.Error("check mobile exist fail, connect db error", err, nil)
		return false
	}

	var count int
	query := fmt.Sprintf("SELECT COUNT(*) FROM userinfo WHERE Mobile = %q", mobile)
	err = db.QueryRowx(query).Scan(&count)
	if err != nil {
		log.Error("check mobile exist fail, db query error", err, map[string]interface{}{
			"sql": query,
		})
		return false
	}

	return count > 0
}
