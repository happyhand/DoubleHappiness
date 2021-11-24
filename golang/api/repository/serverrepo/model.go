package serverrepo

import "api/temp/config"

// #region 後端封包類別資料

const (
	// UserCommandType : user command type
	UserCommandType int = 1
	// RideCommandType : ride command type
	RideCommandType int = 2
	// TeamCommandType : team command type
	TeamCommandType int = 3
)

// #endregion

// Package : 封包內容
type Package struct {
	CmdID int
	Data  interface{}
}

// #region 後端連線資訊

var userConnURL string = config.EnvForge().GetString("SERVER_USER")
var rideConnURL string = config.EnvForge().GetString("SERVER_RIDE")
var teamConnURL string = config.EnvForge().GetString("SERVER_TEAM")

// #endregion
