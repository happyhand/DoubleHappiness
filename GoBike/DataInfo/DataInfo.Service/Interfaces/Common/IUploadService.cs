using DataInfo.Core.Models.Dto.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Common
{
    /// <summary>
    /// 上傳服務
    /// </summary>
    public interface IUploadService
    {
        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UploadImages(IEnumerable<string> imgBase64s);
    }
}