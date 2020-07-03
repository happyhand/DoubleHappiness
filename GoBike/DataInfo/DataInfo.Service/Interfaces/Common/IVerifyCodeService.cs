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
        /// <param name="email">email</param>
        void Delete(string email);

        /// <summary>
        /// 是否已產生驗證碼
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>bool</returns>
        Task<bool> IsGenerate(string email);

        /// <summary>
        /// 生產驗證碼
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>string</returns>
        string Generate(string email);

        /// <summary>
        /// 驗證驗證碼
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="verifierCode">verifierCode</param>
        /// <param name="isDelete">isDelete</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Validate(string email, string verifierCode, bool isDelete);
    }
}