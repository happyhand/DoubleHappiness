package main

import (
	"net/http"
	"tool/service/unittest/auth"
)

func main() {
	auth.TestLogin()

	server := &http.Server{
		Addr: ":18597",
	}

	server.ListenAndServe()
}
