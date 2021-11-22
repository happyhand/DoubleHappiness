package validate

import (
	"errors"
	"fmt"
	"image"
	"io"
	"mime/multipart"

	"github.com/thoas/go-funk"

	// 初始化 gif, 以提供解碼功能
	_ "image/gif"
	// 初始化 jpeg, 以提供解碼功能
	_ "image/jpeg"
	// 初始化 png, 以提供解碼功能
	_ "image/png"
)

// Image : 驗證圖片
func Image(file multipart.File, contentType string, size int64, options ImageOptions) error {
	if !funk.ContainsString(options.Types, contentType) {
		return fmt.Errorf("illegal format type:%s", contentType)
	}

	if size > options.MaxSize {
		return errors.New("file size over than maximum")
	}

	// 編譯圖檔並驗證格式
	_, err := file.Seek(0, io.SeekStart) // 需要將 seek 指回原點，避免檔案操作錯誤
	if err != nil {
		return err
	}

	imgConfig, _, err := image.DecodeConfig(file)
	if err != nil {
		return err
	}

	if options.MaxWidth > 0 && imgConfig.Width != options.MaxWidth {
		return errors.New("illegal image width")
	}

	if options.MaxHeight > 0 && imgConfig.Height != options.MaxHeight {
		return errors.New("illegal image height")
	}

	return nil
}
