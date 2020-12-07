package libs

// Promise :: Promise
type Promise struct {
	done     chan struct{}
	response interface{}
	err      error
}

// DoFunc :: 執行函數
type DoFunc func() (interface{}, error)

// SuccessFunc :: 成功後所執行的函數
type SuccessFunc func(interface{}) (interface{}, error)

// ErrorFunc :: 失敗後所執行的函數
type ErrorFunc func(error) interface{}

// PromiseHandler :: Promise 處理器
func PromiseHandler(doFunc DoFunc) *Promise {
	promise := Promise{done: make(chan struct{})}
	go func() {
		defer close(promise.done)
		promise.response, promise.err = doFunc()
	}()

	return &promise
}

// Done :: 執行 Promise
func (promise *Promise) Done() (interface{}, error) {
	<-promise.done
	return promise.response, promise.err
}

// Then :: Promise 執行延續
func (promise *Promise) Then(successFunc SuccessFunc, errorFunc ErrorFunc) *Promise {
	newPromise := &Promise{done: make(chan struct{})}
	response, err := promise.Done()
	defer close(newPromise.done)
	if err != nil {
		if errorFunc != nil {
			newPromise.response = errorFunc(err)
		} else {
			newPromise.err = err
		}
	} else {
		if successFunc != nil {
			newPromise.response, newPromise.err = successFunc(response)
		} else {
			newPromise.response = response
		}
	}

	return newPromise
}

// Catch :: 捕捉 Promise 例外
func (promise *Promise) Catch(errorFunc ErrorFunc) *Promise {
	return promise.Then(nil, errorFunc)
}
