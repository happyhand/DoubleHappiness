using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Core.Models.Dto.Common.Upload.Request
{
    /// <summary>
    /// 上傳圖像請求資料
    /// </summary>
    public class UploadImageRequest
    {
        /// <summary>
        /// Gets or sets imgBase64s
        /// </summary>
        public IEnumerable<string> ImgBase64s { get; set; }

        /// <summary>
        /// Gets or sets Path
        /// </summary>
        public string Path { get; set; }
    }
}