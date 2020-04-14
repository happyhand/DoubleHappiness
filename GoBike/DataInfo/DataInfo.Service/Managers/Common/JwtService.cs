using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Core.Models.Dto.Common;
using DataInfo.Core.Models.Dto.Response;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Common
{
    /// <summary>
    /// JWT 服務
    /// </summary>
    public class JwtService : IJwtService
    {
        /// <summary>
        /// payloadKey
        /// </summary>
        private static readonly string payloadKey = "Payload";

        /// <summary>
        /// logger
        /// </summary>
        protected readonly ILogger logger = LogManager.GetLogger("JwtService");

        /// <summary>
        /// 生產 Token
        /// </summary>
        /// <param name="jwtClaimsDto">jwtClaimsDto</param>
        /// <returns>string</returns>
        public string GenerateToken(JwtClaimsDto jwtClaimsDto)
        {
            var issuer = AppSettingHelper.Appsetting.Jwt.Iss;
            var signKey = AppSettingHelper.Appsetting.Jwt.Secret;
            var sub = AppSettingHelper.Appsetting.Jwt.Sub;
            var expireMinutes = AppSettingHelper.Appsetting.Jwt.Exp;

            // 設定要加入到 JWT Token 中的聲明資訊(Claims)
            var claims = new List<Claim>
            {
                // 在 RFC 7519 規格中(Section#4)，總共定義了 7 個預設的 Claims，我們應該只用的到兩種！Sub、Jti
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Sub, sub), // User.Identity.Name
                new Claim(JwtRegisteredClaimNames.Aud, issuer),
                //new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()),
                //new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // 必須為數字
                //new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // 必須為數字
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID

                // 網路上常看到的這個 NameId 設定是多餘的
                //new Claim(JwtRegisteredClaimNames.NameId, userName),

                // 這個 Claim 也以直接被 JwtRegisteredClaimNames.Sub 取代，所以也是多餘的
                //new Claim(ClaimTypes.Name, userName),

                // 你可以自行擴充 "roles" 加入登入者該有的角色
                //new Claim("roles", "Admin"),
                //new Claim("roles", "Users")

                #region 新增自定義 Claim

                new Claim(payloadKey, JsonConvert.SerializeObject(jwtClaimsDto))

                #endregion 新增自定義 Claim
            };

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 有要求必須要大於 128 bits，所以 key 不能太短，至少要 16 字元以上 https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                //Audience = issuer, // 由於你的 API 受眾通常沒有區分特別對象，因此通常不太需要設定，也不太需要驗證
                //NotBefore = DateTime.Now, // 預設值就是 DateTime.Now
                //IssuedAt = DateTime.Now, // 預設值就是 DateTime.Now
                Subject = userClaimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }

        /// <summary>
        /// 取得 Payload 指定欄位
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="key">key</param>
        /// <returns>dynamic</returns>
        public dynamic GetPayloadAppointValue(ClaimsPrincipal user, string key)
        {
            try
            {
                string claimJson = user.Claims.Where(data => data.Type.Equals("Payload")).FirstOrDefault().Value;
                JwtClaimsDto jwtClaimsDto = JsonConvert.DeserializeObject<JwtClaimsDto>(claimJson);
                return jwtClaimsDto.GetType().GetProperty(key).GetValue(jwtClaimsDto);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得 Payload 指定欄位發生錯誤", $"Key: {key}", ex);
                return null;
            }
        }
    }
}