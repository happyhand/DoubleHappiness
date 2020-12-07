package apiservice

import (
	"encoding/json"
	"io"
	"io/ioutil"
	"log"
	"net/http"

	apiModel "../../models/api"
)

func init() {
	log.Println("[ApiService] Ready")
}

// RequestHandler :: Api 請求處理器
func RequestHandler(request *http.Request, data interface{}) apiModel.ResponseMessage {
	defer request.Body.Close()
	// 處理 request.Body
	body, err := ioutil.ReadAll(io.LimitReader(request.Body, 1024)) //io.LimitReader限制大小
	if err != nil {
		return GenerateErrorResonse(http.StatusBadRequest, apiModel.InputInvalid)
	}

	// 處理請求資料 json 轉換
	err = json.Unmarshal(body, &data)
	if err != nil {
		return GenerateErrorResonse(http.StatusBadRequest, apiModel.InputInvalid)
	}

	return apiModel.ResponseMessage{Result: true}
}

// ResponseHandler :: Api 回應處理器
func ResponseHandler(write http.ResponseWriter, response apiModel.ResponseMessage) {
	data, err := json.Marshal(response)
	if err != nil {
		ResponseHandler(write, GenerateErrorResonse(http.StatusInternalServerError, apiModel.SystemError))
		return
	}

	write.Header().Set("Content-Type", "application/json")
	write.WriteHeader(response.ResultCode)
	write.Write(data)
}

// GenerateErrorResonse :: 生成 Api 錯誤回應
func GenerateErrorResonse(httpStatus int, message string) apiModel.ResponseMessage {
	return apiModel.ResponseMessage{
		Result:        false,
		ResultCode:    httpStatus,
		ResultMessage: message,
		Content:       nil,
	}
}
