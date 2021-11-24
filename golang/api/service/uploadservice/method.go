package uploadservice

import (
	"fmt"
	"mime/multipart"
	"os"

	"api/core/customerror"

	"api/temp/ants"
	"api/temp/config"
	"api/temp/file/upload"
	"api/temp/file/validate"
	"api/temp/log"
)

// UploadImage : 上傳圖片
func UploadImage(fileHeader *multipart.FileHeader, subDir ...string) (string, *customerror.Error) {
	options := validate.ImageOptions{}
	options.Types = []string{"image/gif", "image/jpeg", "image/png"}
	options.MaxSize = 10 << 20
	filename, err := upload.Image(fileHeader, options, combineDir(subDir...))
	if err != nil {
		log.Error("upload image fila", err, nil)
		return "", customerror.New(customerror.UploadFileError, err.Error())
	}

	return filename, nil
}

// DeleteImage : 刪除圖片
func DeleteImage(filename string, subDir ...string) {
	if filename == "" {
		return
	}

	path := fmt.Sprintf("%s%s", combineDir(subDir...), filename)
	ants.Do(nil, func() {
		err := os.Remove(path)
		if err != nil {
			log.Error("delete image fail", err, map[string]interface{}{
				"filename": filename,
			})
		}
	})
}

// combineDir : 組合資料夾路徑
func combineDir(subDir ...string) string {
	dir := fmt.Sprintf("%s/images/", config.EnvForge().GetString("UPLOAD_DIR"))
	for _, path := range subDir {
		dir += path + "/"
	}

	return dir
}
