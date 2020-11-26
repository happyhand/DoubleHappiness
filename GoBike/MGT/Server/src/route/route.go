package route

import (
	"log"
	"net/http"

	"github.com/gorilla/mux"
)

// RouteManager :: 路由管理器
var RouteManager *mux.Router

func init() {
	RouteManager = mux.NewRouter()
	http.Handle("/", RouteManager)
	log.Println("[Route] Ready")
}

// RegisterAPI :: 註冊路由
func RegisterAPI(path string, action func(http.ResponseWriter,
	*http.Request), methods string) {
	RouteManager.HandleFunc(path, action).Methods(methods)
}
