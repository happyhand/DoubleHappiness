package auth

import (
	"fmt"
	"net/http"

	"tool/log"
	"tool/repository/useraccount"
	"tool/service/api"

	"github.com/chebyrash/promise"
	"github.com/go-resty/resty/v2"
	"github.com/mitchellh/mapstructure"
)

// TestLogin : 測試登入
func TestLogin() {
	users, err := useraccount.GetList()
	if err != nil {
		log.Error("test login fail, not get user list", err, nil)
	}

	for _, user := range users {
		body := map[string]string{
			"email":    user.Email,
			"password": user.Password,
		}

		promise.New(func(resolve func(promise.Any), reject func(error)) {
			var apiResponse api.Response
			response, err := resty.New().R().SetBody(body).SetResult(&apiResponse).Post("http://apigobike.ddns.net:18596/api/Member/Login")

			if err != nil {
				log.Error("test login fail, request api error", err, map[string]interface{}{
					"body": body,
				})
				return
			}

			if response.StatusCode() != http.StatusOK {
				log.Error("test login fail, api response fail", err, map[string]interface{}{
					"response": response,
				})
				return
			}

			var result LoginResult
			err = mapstructure.Decode(apiResponse.Content, &result)
			if err != nil {
				log.Error("test login fail, decode fail", err, map[string]interface{}{
					"Content": apiResponse.Content,
				})
				return
			}

			fmt.Println("============================")
			fmt.Println(result.Token)
		})
	}
}
