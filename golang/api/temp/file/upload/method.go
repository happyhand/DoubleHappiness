package upload

import (
	"fmt"
	"io"
	"mime/multipart"
	"net/http"
	"os"
	"path/filepath"
	"time"

	"api/temp/file/validate"

	"github.com/google/uuid"
)

// Image : 上傳圖片
func Image(fileHeader *multipart.FileHeader, options validate.ImageOptions, dir string) (string, error) {
	// 開啟檔案
	file, err := fileHeader.Open()
	if err != nil {
		return "", err
	}

	defer file.Close()

	// 建立緩沖區
	buf := make([]byte, fileHeader.Size)
	_, err = file.Read(buf)
	if err != nil {
		return "", err
	}

	// 讀取 content-type
	contentType := http.DetectContentType(buf)
	// 驗證圖片
	err = validate.Image(file, contentType, fileHeader.Size, options)
	if err != nil {
		return "", err
	}

	// 轉換檔名
	filename := newFilename(fileHeader.Filename)
	// 儲存檔案
	err = saveFile(file, dir, filename)
	if err != nil {
		return "", err
	}

	return filename, nil
}

// saveFile : 儲存檔案
func saveFile(file multipart.File, dir string, filename string) error {
	_, err := os.Stat(dir)
	if os.IsNotExist(err) {
		err = os.MkdirAll(dir, os.ModePerm)
		if err != nil {
			return err
		}
	}

	out, err := os.Create(fmt.Sprintf("%s/%s", dir, filename))
	if err != nil {
		return err
	}
	defer out.Close()
	_, err = file.Seek(0, io.SeekStart)
	if err != nil {
		return err
	}

	_, err = io.Copy(out, file)
	return err
}

// newFilename : 轉換新檔名
func newFilename(originName string) string {
	return fmt.Sprintf("%s_%s%s", time.Now().UTC().Format("20060102"), uuid.NewString()[:8], filepath.Ext(originName))
}
