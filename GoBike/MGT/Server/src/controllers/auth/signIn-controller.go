package controller

import (
	"log"
	"net/http"

	libs "../../libs"

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
	libs.PromiseHandler(func() (interface{}, error) {
		var content authModel.SignInContent
		response := apiService.RequestHandler(request, &content)
		if response.Result {
			response = authService.SignIn(content)
		}

		apiService.ResponseHandler(writer, response)
		return response, nil
	}).Done()
}
