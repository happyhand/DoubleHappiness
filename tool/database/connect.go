package database

import (
	"fmt"
	"sync"
	"time"

	"tool/log"

	_ "github.com/go-sql-driver/mysql" // 初始化 mysql
	"github.com/jmoiron/sqlx"
)

// 宣告資料庫連線物件和同步鎖
var (
	// poolSlave : SQL 連線物件池
	poolSlave map[string]*sqlx.DB

	// poolMaster : SQL Master 連線物件池
	poolMaster map[string]*sqlx.DB

	// muSlave : 同步鎖
	muSlave *sync.Mutex

	// muMaster : 同步鎖
	muMaster *sync.Mutex
)

func init() {
	poolSlave = make(map[string]*sqlx.DB)
	poolMaster = make(map[string]*sqlx.DB)

	muSlave = &sync.Mutex{}
	muMaster = &sync.Mutex{}
}

// ConnectMaster : 依資料庫名稱取得資料庫連線 (Master)
func ConnectMaster(values map[string]interface{}) (*sqlx.DB, error) {
	// 同步鎖
	muMaster.Lock()
	defer muMaster.Unlock()

	connection := values["DB"].(string)

	if conn, exist := poolMaster[connection]; exist {
		if err := conn.Ping(); err == nil {
			return conn, err
		}

		conn.Close()
	}

	var err error
	poolMaster[connection], err = createConnection(values)
	return poolMaster[connection], err
}

// ConnectionSlave : 依資料庫名稱取得資料庫連線 (Slave)
func ConnectionSlave(values map[string]interface{}) (*sqlx.DB, error) {
	// 同步鎖
	muSlave.Lock()
	defer muSlave.Unlock()

	connection := values["DB"].(string)

	if conn, exist := poolSlave[connection]; exist {
		if err := conn.Ping(); err == nil {
			return conn, err
		}

		conn.Close()
	}

	var err error
	poolSlave[connection], err = createConnection(values)
	return poolSlave[connection], err
}

// createConnection : 建立資料庫連線
func createConnection(values map[string]interface{}) (*sqlx.DB, error) {
	connValue := fmt.Sprintf("%s:%s@tcp(%s:%s)/%s?parseTime=true&charset=%s&loc=%s&timeout=%s",
		values["Account"],
		values["Password"],
		values["IP"],
		values["Port"],
		values["DB"],
		values["Charset"],
		values["Location"],
		values["Timeout"])

	db, err := sqlx.Open("mysql", connValue) // 開啟連線
	if err != nil {
		log.Error("Connection to database failed. err ", err, map[string]interface{}{
			"connValue": connValue,
		})
		return nil, err
	}

	// 設定timeout,最大連線數,閒置
	db.SetConnMaxLifetime(values["MaxLifeTime"].(time.Duration) * time.Second)
	db.SetMaxOpenConns(values["MaxOpenConns"].(int))
	db.SetMaxIdleConns(values["MaxIdleConns"].(int))

	return db, nil
}

// CloseConnection : 結束資料庫連線
func CloseConnection() {
	muSlave.Lock()
	defer muSlave.Unlock()

	for _, v := range poolSlave {
		v.Close()
	}
}
