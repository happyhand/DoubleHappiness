using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace UploadFiles.Service.Interface

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
        /// <param name="projectName">projectName</param>
        /// <param name="typeName">typeName</param>
        /// <param name="imgBase64s">imgs</param>
        /// <returns>imgUrls</returns>
        IEnumerable<string> UploadImages(string projectName, string typeName, IEnumerable<string> imgBase64s);
    }
}