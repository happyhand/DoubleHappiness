using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using UploadFiles.Core.Models.Dto.Image.Content;

namespace UploadFiles.Service.Interfaces

{
    /// <summary>
    /// 檔案上傳服務
    /// </summary>
    public interface IUploadFileService
    {
        /// <summary>
        /// 上傳圖像
        /// </summary>
        /// <param name="httpRequest">httpRequest</param>
        /// <param name="projectName">projectName</param>
        /// <param name="typeName">typeName</param>
        /// <returns>strings</returns>
        Task<IEnumerable<string>> UploadImages(HttpRequest httpRequest, string projectName, string typeName);

        /// <summary>
        /// 上傳圖像
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>imgUrls</returns>
        IEnumerable<string> UploadImages(ImageContent content);
    }
}