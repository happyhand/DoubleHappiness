package validate

// Options : 檔案驗證選項
type Options struct {
	Types   []string
	MaxSize int64
}

// ImageOptions : 圖片驗證選項
type ImageOptions struct {
	Options
	MaxWidth  int
	MaxHeight int
}
