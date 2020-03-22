using DataInfo.Service.Enums;
using DataInfo.Service.Models.Common.Content;
using DataInfo.Service.Models.Common.Data;
using DataInfo.Service.Models.Response;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Common
{
    /// <summary>
    /// 驗證碼服務
    /// </summary>
    public interface IVerifierService
    {
        /// <summary>
        /// 產生驗證碼
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> GenerateVerifierCode(VerifierType type, VerifierCodeContent content);

        /// <summary>
        /// 發送驗證碼
        /// </summary>
        /// <param name="emailContext">emailContext</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> SendVerifierCode(EmailContext emailContext);

        /// <summary>
        /// 驗證碼驗證
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> ValidateVerifierCode(VerifierType type, VerifierCodeContent content);
    }
}