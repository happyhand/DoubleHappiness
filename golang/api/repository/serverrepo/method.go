package serverrepo

import (
	"errors"
	"fmt"

	"api/temp/log"

	"github.com/gorilla/websocket"
	"github.com/mitchellh/mapstructure"
)

// DoAction : 執行後端指令
func DoAction(commandID int, commandType int, data interface{}, receive interface{}) error {
	// 連線後端
	var connURL string
	switch commandType {
	case UserCommandType:
		connURL = userConnURL
	case RideCommandType:
		connURL = rideConnURL
	case TeamCommandType:
		connURL = teamConnURL
	default:
		return errors.New("illegal server command type")
	}

	conn, _, err := websocket.DefaultDialer.Dial(fmt.Sprintf("ws://%s", connURL), nil)
	if err != nil {
		log.Error("connect server fail", err, map[string]interface{}{
			"commandID":   commandID,
			"commandType": commandType,
		})
		return err
	}

	defer conn.Close()
	// 發送後端封包
	request := Package{
		CmdID: commandID,
		Data:  data,
	}
	err = conn.WriteJSON(request)
	if err != nil {
		log.Error("send server request fail", err, map[string]interface{}{
			"commandID":   commandID,
			"commandType": commandType,
			"request":     request,
		})
		return err
	}

	// 接收後端封包
	var response Package
	err = conn.ReadJSON(&response)
	if err != nil {
		log.Error("read server response fail", err, map[string]interface{}{
			"commandID":   commandID,
			"commandType": commandType,
		})
		return err
	}

	// 轉換回應內容
	mapstructure.Decode(response.Data, receive)
	return nil
}
