package service

import (
	"log"
	"net/http"

	responseMessageModel "../../models/api"
	signInModel "../../models/auth"
)

func init() {
	log.Println("[AuthService] Ready")
}

// SignIn :: 會員登入
func SignIn(content signInModel.SignInContent) responseMessageModel.ResponseMessage {
	response := signInModel.SignInResponse{Account: content.Account, Password: content.Password}
	return responseMessageModel.ResponseMessage{Result: true, ResultCode: http.StatusOK, ResultMessage: "Ok", Content: response}
}
