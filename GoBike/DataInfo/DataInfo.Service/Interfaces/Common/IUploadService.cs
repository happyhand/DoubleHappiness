using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataInfo.Service.Models.Response;

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
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> UploadImages(IEnumerable<string> imgBase64s);
    }
}