package controller

import (
	"log"
	"net/http"

	libs "../../libs"

	apiModel "../../models/api"
	authModel "../../models/auth"
	route "../../route"
	apiService "../../service/api"
	authService "../../service/auth"
)

func init() {
	route.RegisterAPI("/api/signin", SignIn, http.MethodPost)
	log.Println("[SignInController] Ready")
}

// SignIn :: Api 接口 - 會員登入
func SignIn(writer http.ResponseWriter,
	request *http.Request) {
	promise := libs.PromiseHandler(func() (interface{}, error) {
		// do work asynchronously here
		var content authModel.SignInContent
		err := apiService.RequestHandler(request, &content)
		var response apiModel.ResponseMessage
		if err == nil {
			response = authService.SignIn(content)
		} else {
			response = apiService.GenerateErrorResonse(http.StatusInternalServerError, apiModel.SystemError)
		}

		apiService.ResponseHandler(writer, response)
		return response, err
	})

	promise.Then()
}
