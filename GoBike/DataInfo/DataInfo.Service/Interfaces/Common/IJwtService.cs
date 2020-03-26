﻿using DataInfo.Service.Models.Common.Data;
using DataInfo.Service.Models.Response;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
        /// <param name="jwtClaimsDto">jwtClaimsDto</param>
        /// <returns>string</returns>
        string GenerateToken(JwtClaimsDto jwtClaimsDto);

        /// <summary>
        /// 取得 Payload 指定欄位
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="key">key</param>
        /// <returns>dynamic</returns>
        dynamic GetPayloadAppointValue(ClaimsPrincipal user, string key);
    }
}