package main

import (
	"context"
	"log"
	"net/http"
	"os"
	"os/signal"

	_ "./controllers/auth"
	route "./route"
)

// TODO 移至設定檔
const (
	port string = ":3000"
)

func main() {
	log.Println("[Main] Start")
	StartServer()
}

// StartServer :: Start Server
func StartServer() {
	// server
	server := http.Server{Addr: port,
		Handler: route.RouteManager}

	// make sure idle connections returned
	processed := make(chan int)
	go func() {
		c := make(chan os.Signal, 1)
		signal.Notify(c, os.Interrupt)
		<-c

		ctx, cancel := context.WithCancel(context.Background())
		defer cancel()
		err := server.Shutdown(ctx)
		if err != nil {
			log.Fatalf("server shutdown failed, err: %v\n", err)
		}

		log.Println("server gracefully shutdown")
		close(processed)
	}()

	// serve
	err := server.ListenAndServe()
	if http.ErrServerClosed != err {
		log.Fatalf("server not gracefully shutdown, err :%v\n", err)
	}

	// waiting for goroutine above processed
	<-processed
}
