package controller

import (
	"fmt"
	"io"
	"net/http"
	"net/url"

	"api/temp/api"

	"github.com/go-resty/resty/v2"
)

const (
	// Domain : nezuko api domain
	Domain string = "http://localhost:18600/"
)

// SendAPI : send api
func SendAPI(path string, method string, appendToken bool, params interface{}, files map[string]map[string]io.Reader) (*api.ResponseBody, error) {
	var (
		result   api.ResponseBody
		response *resty.Response
		err      error
	)

	request := resty.New().R()
	if appendToken {
		resp, err := SendAPI(Domain+"api/member/login", http.MethodPost, false, url.Values{
			"Email":    []string{"robot0001@gmail.com"},
			"Password": []string{"a123456"},
		}, nil)

		if err != nil {
			return nil, err
		}

		token := resp.Content
		request.SetAuthToken(token.(string))
		for key, file := range files {
			for filename, reader := range file {
				request.SetFileReader(key, filename, reader)
			}
		}
	}

	request.SetResult(&result)
	switch method {
	case http.MethodPost:
		response, err = request.SetFormDataFromValues(params.(url.Values)).Post(path)
	case http.MethodGet:
		response, err = request.SetQueryParams(params.(map[string]string)).Get(path)
	case http.MethodDelete:
		response, err = request.SetBody(params).Delete(path)
	}

	if response.StatusCode() != http.StatusOK {
		return nil, fmt.Errorf("api response fail(status code:%d result:%v)", response.StatusCode(), string(response.Body()))
	}

	return &result, err
}
