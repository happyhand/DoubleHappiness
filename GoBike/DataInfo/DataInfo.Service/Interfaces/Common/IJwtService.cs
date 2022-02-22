using DataInfo.Core.Models.Dto.Common;
using System.Security.Claims;

namespace DataInfo.Service.Interfaces.Common
{
    /// <summary>
    /// JWT 服務
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// 生產 Token
        /// </summary>
        /// <param name="jwtClaims">jwtClaims</param>
        /// <param name="isRefreshToken">isRefreshToken</param>
        /// <returns>string</returns>
        string GenerateToken(JwtClaims jwtClaims, bool isRefreshToken);

        /// <summary>
        /// 取得 Payload 指定欄位
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="key">key</param>
        /// <returns>dynamic</returns>
        dynamic GetPayloadAppointValue(ClaimsPrincipal user, string key);
    }
}