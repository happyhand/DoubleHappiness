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
        /// Gets or sets Project
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets Type
        /// </summary>
        public string Type { get; set; }
    }
}