using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Service.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Common
{
    /// <summary>
    /// 驗證碼服務
    /// </summary>
    public class VerifyCodeService : IVerifyCodeService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("VerifyCodeService");

        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="redisRepository">redisRepository</param>
        public VerifyCodeService(IRedisRepository redisRepository)
        {
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 刪除驗證碼
        /// </summary>
        /// <param name="email">email</param>
        public void Delete(string email)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{email}";
                this.redisRepository.DeleteCache(AppSettingHelper.Appsetting.Redis.CommonDB, cacheKey);
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除驗證碼發生錯誤", $"Email: {email}", ex);
            }
        }

        /// <summary>
        /// 生產驗證碼
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>string</returns>
        public string Generate(string email)
        {
            try
            {
                string verifierCode = Guid.NewGuid().ToString().Substring(0, AppSettingHelper.Appsetting.VerifierCode.Length);
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{email}";
                this.redisRepository.SetCache(AppSettingHelper.Appsetting.Redis.CommonDB, cacheKey, JsonConvert.SerializeObject(verifierCode), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.VerifierCode.ExpirationDate));
                return verifierCode;
            }
            catch (Exception ex)
            {
                this.logger.LogError("生產驗證碼發生錯誤", $"Email: {email}", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 是否已產生驗證碼
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>bool</returns>
        public async Task<bool> IsGenerate(string email)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{email}";
                return await this.redisRepository.IsExist(AppSettingHelper.Appsetting.Redis.CommonDB, cacheKey, false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("是否已產生驗證碼發生錯誤", $"Email: {email}", ex);
                return false;
            }
        }

        /// <summary>
        /// 驗證驗證碼
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="verifierCode">verifierCode</param>
        /// <param name="isDelete">isDelete</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Validate(string email, string verifierCode, bool isDelete)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{email}";
                string matchCode = await this.redisRepository.GetCache<string>(AppSettingHelper.Appsetting.Redis.CommonDB, cacheKey).ConfigureAwait(false);
                bool result = verifierCode.Equals(matchCode);
                if (result && isDelete)
                {
                    this.Delete(verifierCode);
                }

                return new ResponseResult()
                {
                    Result = result,
                    ResultCode = result ? StatusCodes.Status200OK : StatusCodes.Status409Conflict,
                    ResultMessage = result ? string.Empty : ResponseErrorMessageType.VerifyCodeFail.ToString()
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("驗證驗證碼發生錯誤", $"Email: {email} VerifierCode: {verifierCode} IsDelete: {isDelete}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }
    }
}