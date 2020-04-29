using DataInfo.Core.Models.Dto.Common;
using DataInfo.Core.Models.Dto.Response;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Common
{
    /// <summary>
    /// 驗證碼服務
    /// </summary>
    public interface IVerifyCodeService
    {
        /// <summary>
        /// 刪除驗證碼
        /// </summary>
        /// <param name="verifierCode">verifierCode</param>
        void Delete(string verifierCode);

        /// <summary>
        /// 生產驗證碼
        /// </summary>
        /// <returns>string</returns>
        Task<string> Generate();

        /// <summary>
        /// 驗證驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="isDelete">isDelete</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Validate(VerifyCodeContent content, bool isDelete);

        /// <summary>
        /// 驗證驗證碼
        /// </summary>
        /// <param name="verifierCode">verifierCode</param>
        /// <param name="isDelete">isDelete</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Validate(string verifierCode, bool isDelete);
    }
}