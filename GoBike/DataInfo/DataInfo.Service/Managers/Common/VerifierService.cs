using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Service.Enums;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Models.Common.Content;
using DataInfo.Service.Models.Common.Data;
using DataInfo.Service.Models.Response;
using FluentValidation.Results;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Common
{
    /// <summary>
    /// 驗證碼服務
    /// </summary>
    public class VerifierService : IVerifierService
    {
        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// logger
        /// </summary>
        protected readonly ILogger logger = LogManager.GetLogger("VerifierService");

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="redisRepository">redisRepository</param>
        public VerifierService(IRedisRepository redisRepository)
        {
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 產生驗證碼
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> GenerateVerifierCode(VerifierType type, VerifierCodeContent content)
        {
            try
            {
                #region 驗證登入資料

                VerifierCodeContentValidator verifierCodeContentValidator = new VerifierCodeContentValidator(false);
                ValidationResult validationResult = verifierCodeContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("產生驗證碼結果", $"Result: 驗證失敗({errorMessgae}) Type: {type} content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證登入資料

                #region 產生驗證碼

                string verifierCode = Guid.NewGuid().ToString().Substring(0, 6);
                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.VerifierCode}-{type}-{content.Email}";
                this.redisRepository.SetCache(cacheKey, verifierCode, TimeSpan.FromMinutes(AppSettingHelper.Appsetting.VerifierCodeExpirationDate));

                #endregion 產生驗證碼

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = verifierCode
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("產生驗證碼發生錯誤", $"Type: {type} content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "驗證發生錯誤."
                };
            }
        }

        /// <summary>
        /// 發送驗證碼
        /// </summary>
        /// <param name="emailContext">emailContext</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> SendVerifierCode(EmailContext emailContext)
        {
            try
            {
                string postData = JsonConvert.SerializeObject(emailContext);
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.SmtpServer.Domain, AppSettingHelper.Appsetting.SmtpServer.SendEmailApi, postData);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("發送驗證碼結果", $"Result: 發送郵件失敗({httpResponseMessage.Content}) EmailContext: {JsonConvert.SerializeObject(emailContext)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = "發送驗證碼失敗."
                    };
                }

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "已發送驗證碼."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送驗證碼發生錯誤", $"EmailContext: {JsonConvert.SerializeObject(emailContext)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "發送驗證碼發生錯誤."
                };
            }
        }

        /// <summary>
        /// 驗證碼驗證
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> ValidateVerifierCode(VerifierType type, VerifierCodeContent content)
        {
            try
            {
                #region 驗證登入資料

                VerifierCodeContentValidator verifierCodeContentValidator = new VerifierCodeContentValidator(true);
                ValidationResult validationResult = verifierCodeContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("驗證碼驗證結果", $"Result: 驗證失敗({errorMessgae}) Type: {type} content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證登入資料

                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.VerifierCode}-{type}-{content.Email}-{content.VerifierCode}";
                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = await this.redisRepository.IsExist(cacheKey).ConfigureAwait(false)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("驗證碼驗證發生錯誤", $"Type: {type} content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "驗證發生錯誤."
                };
            }
        }
    }
}