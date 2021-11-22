package database

import (
	"fmt"
	"sync"
	"time"

	"api/temp/log"

	_ "github.com/go-sql-driver/mysql" // 初始化 mysql
	"github.com/jmoiron/sqlx"
)

// 宣告資料庫連線物件和同步鎖
var (
	// poolSlave : SQL 連線物件池
	poolSlave map[string]*sqlx.DB

	// poolMaster : SQL Master 連線物件池
	poolMaster map[string]*sqlx.DB

	// muSlave : slave 同步鎖
	muSlave *sync.Mutex

	// muMaster : master 同步鎖
	muMaster *sync.Mutex
)

func init() {
	poolSlave = make(map[string]*sqlx.DB)
	poolMaster = make(map[string]*sqlx.DB)

	muSlave = &sync.Mutex{}
	muMaster = &sync.Mutex{}
}

// ConnectMaster : 依資料庫名稱取得資料庫連線 (Master)
func ConnectMaster(config ConnectConfig) (*sqlx.DB, error) {
	// 同步鎖
	muMaster.Lock()
	defer muMaster.Unlock()
	if db, exist := poolMaster[config.DB]; exist {
		if err := db.Ping(); err == nil {
			return db, err
		}
		db.Close()
	}

	var err error
	poolMaster[config.DB], err = createConnection(config)
	return poolMaster[config.DB], err
}

// ConnectSlave : 依資料庫名稱取得資料庫連線 (Slave)
func ConnectSlave(config ConnectConfig) (*sqlx.DB, error) {
	// 同步鎖
	muSlave.Lock()
	defer muSlave.Unlock()
	if db, exist := poolSlave[config.DB]; exist {
		if err := db.Ping(); err == nil {
			return db, err
		}
		db.Close()
	}

	var err error
	poolSlave[config.DB], err = createConnection(config)
	return poolSlave[config.DB], err
}

// createConnection : 建立資料庫連線
func createConnection(config ConnectConfig) (*sqlx.DB, error) {
	connect := fmt.Sprintf("%s:%s@tcp(%s:%s)/%s?parseTime=true&charset=%s&loc=%s&timeout=%s",
		config.Account,
		config.Password,
		config.IP,
		config.Port,
		config.DB,
		config.Charset,
		config.Location,
		config.Timeout)

	db, err := sqlx.Open("mysql", connect) // 開啟連線
	if err != nil {
		log.Error("connection to database fail", err, map[string]interface{}{
			"connect": connect,
		})
		return nil, err
	}

	// 設定timeout,最大連線數,閒置
	db.SetConnMaxLifetime(time.Duration(config.MaxLifeTime) * time.Second)
	db.SetMaxOpenConns(config.MaxOpenConns)
	db.SetMaxIdleConns(config.MaxIdleConns)

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
