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
        /// 上傳會員圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        Task<IEnumerable<string>> UploadMemberImages(IEnumerable<string> imgBase64s, bool isIgnoreUri);

        /// <summary>
        /// 上傳騎乘圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        Task<IEnumerable<string>> UploadRideImages(IEnumerable<string> imgBase64s, bool isIgnoreUri);

        /// <summary>
        /// 上傳車隊活動圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        Task<IEnumerable<string>> UploadTeamActivityImages(IEnumerable<string> imgBase64s, bool isIgnoreUri);

        /// <summary>
        /// 上傳車隊圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        Task<IEnumerable<string>> UploadTeamImages(IEnumerable<string> imgBase64s, bool isIgnoreUri);
    }
}