using System.Collections.Generic;

namespace UploadFiles.Core.Models.Dto.Image.Content
{
    /// <summary>
    /// 圖像上傳內容
    /// </summary>
    public class ImageContent
    {
        /// <summary>
        /// Gets or sets ImgBase64s
        /// </summary>
        public IEnumerable<string> ImgBase64s { get; set; }

        /// <summary>
        /// Gets or sets Path
        /// </summary>
        public string Path { get; set; }
    }
}