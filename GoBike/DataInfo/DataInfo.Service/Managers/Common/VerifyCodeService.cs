using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Common;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Service.Interfaces.Common;
using FluentValidation.Results;
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
        /// <param name="verifierCode">verifierCode</param>
        public void Delete(string verifierCode)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{verifierCode}";
                this.redisRepository.DeleteCache(cacheKey);
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除驗證碼發生錯誤", $"VerifierCode: {verifierCode}", ex);
            }
        }

        /// <summary>
        /// 生產驗證碼
        /// </summary>
        /// <returns>string</returns>
        public async Task<string> Generate()
        {
            try
            {
                string verifierCode = Guid.NewGuid().ToString().Substring(0, 6);
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{verifierCode}";
                bool isExist = await this.redisRepository.IsExist(cacheKey).ConfigureAwait(false);
                if (isExist)
                {
                    return await this.Generate().ConfigureAwait(false);
                }

                this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(verifierCode), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.VerifierCodeExpirationDate));
                return verifierCode;
            }
            catch (Exception ex)
            {
                this.logger.LogError("生產驗證碼發生錯誤", string.Empty, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 驗證驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="isDelete">isDelete</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Validate(VerifyCodeContent content, bool isDelete)
        {
            try
            {
                #region 驗證資料

                VerifyCodeContentValidator verifyCodeContentValidator = new VerifyCodeContentValidator();
                ValidationResult validationResult = verifyCodeContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("驗證驗證碼結果", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)} IsDelete: {isDelete}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                return await this.Validate(content.VerifierCode, isDelete);
            }
            catch (Exception ex)
            {
                this.logger.LogError("驗證驗證碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)} IsDelete: {isDelete}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.VerifyCode.MatchFail
                };
            }
        }

        /// <summary>
        /// 驗證驗證碼
        /// </summary>
        /// <param name="verifierCode">verifierCode</param>
        /// <param name="isDelete">isDelete</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Validate(string verifierCode, bool isDelete)
        {
            try
            {
                if (string.IsNullOrEmpty(verifierCode))
                {
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = MessageHelper.Message.ResponseMessage.VerifyCode.MatchFail
                    };
                }

                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{verifierCode}";
                string matchCode = await this.redisRepository.GetCache<string>(cacheKey).ConfigureAwait(false);
                bool result = verifierCode.Equals(matchCode);
                if (result && isDelete)
                {
                    this.Delete(verifierCode);
                }

                return new ResponseResult()
                {
                    Result = result,
                    ResultCode = result ? (int)ResponseResultType.Success : (int)ResponseResultType.InputError,
                    Content = result ? MessageHelper.Message.ResponseMessage.VerifyCode.MatchSuccess : MessageHelper.Message.ResponseMessage.VerifyCode.MatchFail
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("驗證驗證碼發生錯誤", $"VerifierCode: {verifierCode} IsDelete: {isDelete}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.VerifyCode.MatchFail
                };
            }
        }
    }
}