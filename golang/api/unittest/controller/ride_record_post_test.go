package controller

import (
	"bytes"
	"io"
	"io/ioutil"
	"net/http"
	"net/url"
	"testing"
)

// TestRideRecordPost : test ride RecordPost
func TestRideRecordPost(t *testing.T) {
	imgBytes1, _ := ioutil.ReadFile("resource/image.png")
	imgBytes2, _ := ioutil.ReadFile("resource/image2.png")
	imgBytes3, _ := ioutil.ReadFile("resource/image3.png")
	// data := map[string]string{
	// 	"ShareContent": "",
	// }

	files := map[string]map[string]io.Reader{
		"ShareImage": {
			"a": bytes.NewReader(imgBytes1),
			"b": bytes.NewReader(imgBytes2),
			"c": bytes.NewReader(imgBytes3),
		},
	}
	path := Domain + "api/ride/record"
	_, err := SendAPI(path, http.MethodPost, true, url.Values{
		"ShareContent": []string{"a", "b", "c"},
	}, files)

	if err != nil {
		t.Fatalf("TestBackADsPost fail >>> err: %s", err.Error())
	}
}
