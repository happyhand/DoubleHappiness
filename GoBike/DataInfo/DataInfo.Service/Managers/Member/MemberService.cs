using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Models.Member;
using DataInfo.Service.Enums;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Models.Common.Data;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Member.View;
using DataInfo.Service.Models.Response;
using FluentValidation.Results;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Member
{
    /// <summary>
    /// 會員服務
    /// </summary>
    public class MemberService : IMemberService
    {
        /// <summary>
        /// jwtService
        /// </summary>
        private readonly IJwtService jwtService;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberService");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        private readonly IMemberRepository memberRepository;

        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// uploadService
        /// </summary>
        private readonly IUploadService uploadService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="jwtService">jwtService</param>
        /// <param name="uploadService">uploadService</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public MemberService(IMapper mapper, IJwtService jwtService, IUploadService uploadService, IMemberRepository memberRepository, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.jwtService = jwtService;
            this.uploadService = uploadService;
            this.memberRepository = memberRepository;
            this.redisRepository = redisRepository;
        }

        #region 註冊 \ 登入 \ 登出 \ 保持在線

        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>MemberModel</returns>
        private MemberModel CreateMemberModel(string email, string password, string fbToken, string googleToken)
        {
            byte[] dateTimeBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
            Array.Resize(ref dateTimeBytes, 16);
            string memberID = $"{AppSettingHelper.Appsetting.MemberIDFlag}{new Guid(dateTimeBytes).ToString().Substring(0, 8)}";
            return new MemberModel()
            {
                MemberID = memberID,
                RegisterDate = DateTime.UtcNow,
                RegisterSource = string.IsNullOrEmpty(fbToken) ? string.IsNullOrEmpty(googleToken) ? (int)RegisterSourceType.Normal : (int)RegisterSourceType.Google : (int)RegisterSourceType.FB,
                Email = email,
                Password = string.IsNullOrEmpty(password) ? string.Empty : Utility.EncryptAES(password),
                FBToken = fbToken,
                GoogleToken = googleToken,
            };
        }

        /// <summary>
        /// 生產 Jwt Token
        /// </summary>
        /// <param name="memberModel">memberModel</param>
        /// <returns>string</returns>
        private string GenerateJwtToken(MemberModel memberModel)
        {
            JwtClaimsDto jwtClaimsDto = new JwtClaimsDto()
            {
                MemberID = memberModel.MemberID,
                Email = memberModel.Email,
                Nickname = memberModel.Nickname,
                Avatar = memberModel.Avatar,
                FrontCover = memberModel.FrontCover,
                Mobile = memberModel.Mobile
            };
            return this.jwtService.GenerateToken(jwtClaimsDto);
        }

        /// <summary>
        /// 更新最新登入時間
        /// </summary>
        /// <param name="memberModel">memberModel</param>
        private void UpdateLastLoginDate(MemberModel memberModel)
        {
            try
            {
                this.logger.LogInfo("更新會員最新登入時間", $"MemberID: {memberModel.MemberID}", null);
                memberModel.LoginDate = DateTime.UtcNow;
                this.memberRepository.Update(memberModel);

                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-login-{memberModel.MemberID}";
                this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(DateTime.UtcNow), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.KeepOnlineTime));
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新會員最新登入時間發生錯誤", $"MemberID: {memberModel.MemberID}", ex);
            }
        }

        /// <summary>
        /// 會員保持在線
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> KeepOnline(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-login-{memberID}";
                bool result = await this.redisRepository.UpdateCacheExpire(cacheKey, TimeSpan.FromMinutes(AppSettingHelper.Appsetting.KeepOnlineTime)).ConfigureAwait(false);
                if (result)
                {
                    return new ResponseResultDto()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = "保持在線成功."
                    };
                }

                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UpdateFail,
                    Content = "保持在線失敗."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員保持在線發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "保持在線發生錯誤."
                };
            }
        }

        /// <summary>
        /// 會員登入(一般登入)
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Login(MemberLoginContent content)
        {
            try
            {
                #region 驗證資料

                MemberLoginContentValidator memberLoginContentValidator = new MemberLoginContentValidator();
                ValidationResult validationResult = memberLoginContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("會員登入結果(一般登入)", $"Result: 驗證失敗({errorMessgae}) Email: {content.Email} Password: {content.Password}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 取得資料並驗證密碼

                MemberModel memberModel = await this.GetMemberData(content.Email).ConfigureAwait(false);
                if (memberModel == null)
                {
                    this.logger.LogWarn("會員登入結果(一般登入)", $"Result: 無會員資料 Email: {content.Email} Password: {content.Password}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "無法根據信箱查詢到相關會員."
                    };
                }

                string decryptAESPassword = Utility.DecryptAES(memberModel.Password);
                if (!decryptAESPassword.Equals(content.Password))
                {
                    this.logger.LogWarn("會員登入結果(一般登入)", $"Result: 密碼驗證失敗 Email: {content.Email} Password: {content.Password}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "密碼驗證失敗."
                    };
                }

                #endregion 取得資料並驗證密碼

                #region 登入 Firebase (TODO)

                bool firebaseLoginResult = true;
                if (!firebaseLoginResult)
                {
                    this.logger.LogWarn("會員登入結果(一般登入)", $"Result: Firebase 登入失敗 Email: {content.Email} Password: {content.Password}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UnknownError,
                        Content = "登入失敗."
                    };
                }

                #endregion 登入 Firebase (TODO)

                #region 更新最新登入時間

                this.UpdateLastLoginDate(memberModel);

                #endregion 更新最新登入時間

                this.logger.LogInfo("會員登入成功(一般登入)", $"Email: {content.Email} Password: {content.Password}", null);
                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = new MemberLoginViewDto() { Token = this.GenerateJwtToken(memberModel) }
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員登入發生錯誤(一般登入)", $"Email: {content.Email} Password: {content.Password}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "登入發生錯誤."
                };
            }
        }

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="isValidatePassword">isValidatePassword</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Register(MemberRegisterContent content, bool isValidatePassword, string fbToken, string googleToken)
        {
            try
            {
                #region 驗證資料

                MemberRegisterContentValidator memberRegisterContentValidator = new MemberRegisterContentValidator(isValidatePassword);
                ValidationResult validationResult = memberRegisterContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("會員註冊結果", $"Result: 驗證失敗({errorMessgae}) Email: {content.Email} Password: {content.Password} ConfirmPassword: {content.ConfirmPassword} IsValidatePassword: {isValidatePassword} FbToken: {fbToken} GoogleToken: {googleToken}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 檢查 Email 是否已被註冊

                MemberModel memberModel = await this.GetMemberData(content.Email).ConfigureAwait(false);
                if (memberModel != null)
                {
                    this.logger.LogWarn("會員註冊結果", $"Result: 此信箱已經被註冊 Email: {content.Email} Password: {content.Password} ConfirmPassword: {content.ConfirmPassword} IsValidatePassword: {isValidatePassword} FbToken: {fbToken} GoogleToken: {googleToken}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Existed,
                        Content = "此信箱已經被註冊."
                    };
                }

                #endregion 檢查 Email 是否已被註冊

                #region 建立 DB 會員資料

                memberModel = this.CreateMemberModel(content.Email, content.Password, fbToken, googleToken);
                bool dbCreateResult = await this.memberRepository.Create(memberModel).ConfigureAwait(false);
                if (!dbCreateResult)
                {
                    this.logger.LogWarn("會員註冊結果", $"Result: DB 建立資料失敗 Email: {content.Email} Password: {content.Password} ConfirmPassword: {content.ConfirmPassword} IsValidatePassword: {isValidatePassword} FbToken: {fbToken} GoogleToken: {googleToken}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.CreateFail,
                        Content = "註冊失敗."
                    };
                }

                #endregion 建立 DB 會員資料

                #region 建立 Firebase 會員資料 (TODO)

                bool firebaseCreateResult = true;
                if (!firebaseCreateResult)
                {
                    this.logger.LogWarn("會員註冊結果", $"Result: Firebase 建立資料失敗 Email: {content.Email} Password: {content.Password} ConfirmPassword: {content.ConfirmPassword} IsValidatePassword: {isValidatePassword} FbToken: {fbToken} GoogleToken: {googleToken}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.CreateFail,
                        Content = "註冊失敗."
                    };
                }

                #endregion 建立 Firebase 會員資料 (TODO)

                this.logger.LogInfo("會員註冊成功", $"Email: {content.Email} Password: {content.Password} ConfirmPassword: {content.ConfirmPassword} IsValidatePassword: {isValidatePassword} FbToken: {fbToken} GoogleToken: {googleToken}", null);
                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "註冊成功."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員註冊發生錯誤", $"Email: {content.Email} Password: {content.Password} ConfirmPassword: {content.ConfirmPassword} IsValidatePassword: {isValidatePassword} FbToken: {fbToken} GoogleToken: {googleToken}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "註冊發生錯誤."
                };
            }
        }

        /// <summary>
        /// 會員登入(重新登入)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Relogin(string memberID)
        {
            try
            {
                #region 取得資料

                MemberModel memberModel = await this.GetMemberData(memberID).ConfigureAwait(false);
                if (memberModel == null)
                {
                    this.logger.LogError("會員登入結果(重新登入)", $"Result: 無會員資料，須查詢 DB 比對 MemberID: {memberID}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = "無會員資料，無法重新登入."
                    };
                }

                #endregion 取得資料

                #region 登入 Firebase (TODO)

                bool firebaseLoginResult = true;
                if (!firebaseLoginResult)
                {
                    this.logger.LogWarn("會員登入結果(重新登入)", $"Result: Firebase 登入失敗 Email: {memberModel.Email} Password: {memberModel.Password}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UnknownError,
                        Content = "登入失敗."
                    };
                }

                #endregion 登入 Firebase (TODO)

                #region 更新最新登入時間

                this.UpdateLastLoginDate(memberModel);

                #endregion 更新最新登入時間

                this.logger.LogInfo("會員登入成功(重新登入)", $"MemberID: {memberID}", null);
                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = new MemberLoginViewDto() { Token = this.GenerateJwtToken(memberModel) }
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員登入發生錯誤(重新登入)", $"MemberID: {memberID}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "登入發生錯誤."
                };
            }
        }

        ///// <summary>
        ///// 會員登入(FB登入)
        ///// </summary>
        ///// <param name="email">email</param>
        ///// <param name="token">token</param>
        ///// <returns>ResponseResultDto</returns>
        //public async Task<ResponseResultDto> LoginWithFB(string email, string token)
        //{
        //    try
        //    {
        //        #region 驗證登入資料

        // if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token)) {
        // this.logger.LogWarn("會員登入結果(FB登入)", $"Result: 驗證失敗 Email: {email} Token: {token}", null);
        // return new ResponseResultDto() { Ok = false, Data = "信箱或認證碼無效." }; }

        // #endregion 驗證登入資料

        // #region 檢查資料是否存在，若不存在則自動註冊資料

        // MemberData memberData = await
        // this.GetMemberData($"{CommonFlagHelper.CommonFlag.PlatformFlag.FB}_{token}").ConfigureAwait(false);
        // if (memberData == null) { ResponseResultDto registerResult = await this.Register(email,
        // string.Empty, false, token, string.Empty); if (registerResult.Ok) { return await
        // this.LoginWithFB(email, token); } else { return registerResult; } }

        // #endregion 檢查資料是否存在，若不存在則自動註冊資料

        // #region 更新最新登入時間

        // this.UpdateLastLoginDate(memberData);

        // #endregion 更新最新登入時間

        //        this.logger.LogInfo("會員登入成功(FB登入)", $"Email: {email} Token: {token}", null);
        //        return new ResponseResultDto()
        //        {
        //            Ok = true,
        //            Data = new MemberLoginInfoViewDto { MemberID = memberData.MemberID }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("會員登入發生錯誤(FB登入)", $"Email: {email} Token: {token}", ex);
        //        return new ResponseResultDto()
        //        {
        //            Ok = false,
        //            Data = "會員登入發生錯誤."
        //        };
        //    }
        //}

        ///// <summary>
        ///// 會員登入(Google登入)
        ///// </summary>
        ///// <param name="email">email</param>
        ///// <param name="token">token</param>
        ///// <returns>ResponseResultDto</returns>
        //public async Task<ResponseResultDto> LoginWithGoogle(string email, string token)
        //{
        //    try
        //    {
        //        #region 驗證登入資料

        // if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token)) {
        // this.logger.LogWarn("會員登入結果(Google登入)", $"Result: 驗證失敗 Email: {email} Token: {token}",
        // null); return new ResponseResultDto() { Ok = false, Data = "信箱或認證碼無效." }; }

        // #endregion 驗證登入資料

        // #region 檢查資料是否存在，若不存在則自動註冊資料

        // MemberData memberData = await
        // this.GetMemberData($"{CommonFlagHelper.CommonFlag.PlatformFlag.Google}_{token}").ConfigureAwait(false);
        // if (memberData == null) { ResponseResultDto registerResult = await this.Register(email,
        // string.Empty, false, string.Empty, token); if (registerResult.Ok) { return await
        // this.LoginWithGoogle(email, token); } else { return registerResult; } }

        // #endregion 檢查資料是否存在，若不存在則自動註冊資料

        // #region 更新最新登入時間

        // this.UpdateLastLoginDate(memberData);

        // #endregion 更新最新登入時間

        //        this.logger.LogInfo("會員登入成功(Google登入)", $"Email: {email} Token: {token}", null);
        //        return new ResponseResultDto()
        //        {
        //            Ok = true,
        //            Data = new MemberLoginInfoViewDto { MemberID = memberData.MemberID }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("會員登入發生錯誤(Google登入)", $"Email: {email} Token: {token}", ex);
        //        return new ResponseResultDto()
        //        {
        //            Ok = false,
        //            Data = "會員登入發生錯誤."
        //        };
        //    }
        //}

        ///// <summary>
        ///// 會員登出
        ///// </summary>
        ///// <param name="memberID">memberID</param>
        //public void Logout(string memberID)
        //{
        //    try
        //    {
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("會員登出發生錯誤", $"MemberID: {memberID}", ex);
        //    }
        //}

        #endregion 註冊 \ 登入 \ 登出 \ 保持在線

        #region 會員資料

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <returns>MemberModel</returns>
        private async Task<MemberModel> GetMemberData(string searchKey)
        {
            try
            {
                if (searchKey.Contains("@"))
                {
                    return await this.memberRepository.GetByEmail(searchKey);
                }
                else if (searchKey.Contains(AppSettingHelper.Appsetting.MemberIDFlag))
                {
                    return await this.memberRepository.GetByMemberID(searchKey);
                }
                //else if (searchKey.Contains("FB"))
                //{
                //    searchKey = searchKey.Replace("FB_", string.Empty);
                //    return this.memberRepository.GetMemberDataByFB(searchKey);
                //}
                //else if (searchKey.Contains("Google"))
                //{
                //    searchKey = searchKey.Replace("Google_", string.Empty);
                //    return this.memberRepository.GetMemberDataByGoogle(searchKey);
                //}

                return null;
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤", $"SearchKey: {searchKey}", ex);
                return null;
            }
        }

        /// <summary>
        /// 會員資料更新處理
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="model">model</param>
        /// <returns>string</returns>
        private async Task<string> UpdateInfoHandler(MemberEditInfoContent content, MemberModel model)
        {
            if (!string.IsNullOrEmpty(content.Avatar) || !string.IsNullOrEmpty(content.FrontCover))
            {
                List<string> imgBase64s = new List<string>() { content.Avatar, content.FrontCover };
                ResponseResultDto uploadResponseResult = await this.uploadService.UploadImages(imgBase64s).ConfigureAwait(false);
                if (!uploadResponseResult.Result)
                {
                    return "上傳圖片失敗.";
                }

                IEnumerable<string> imgUrls = uploadResponseResult.Content;
                if (!string.IsNullOrEmpty(content.Avatar))
                {
                    string avatar = imgUrls.ElementAt(0);
                    if (string.IsNullOrEmpty(avatar))
                    {
                        return "上傳頭像失敗.";
                    }

                    model.Avatar = avatar;
                }

                if (!string.IsNullOrEmpty(content.FrontCover))
                {
                    string frontCover = imgUrls.ElementAt(1);
                    if (string.IsNullOrEmpty(frontCover))
                    {
                        return "上傳封面失敗.";
                    }

                    model.FrontCover = frontCover;
                }
            }

            if (content.Birthday.HasValue)
            {
                model.Birthday = TimeZoneInfo.ConvertTimeToUtc(content.Birthday.Value);
            }

            if (content.BodyHeight > 0)
            {
                model.BodyHeight = content.BodyHeight;
            }

            if (content.BodyWeight > 0)
            {
                model.BodyWeight = content.BodyWeight;
            }

            if (content.Gender != (int)GenderType.None)
            {
                model.Gender = content.Gender;
            }

            if (!string.IsNullOrEmpty(content.Nickname))
            {
                model.Nickname = content.Nickname;
            }

            return string.Empty;
        }

        /// <summary>
        /// 會員編輯資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> EditInfo(string memberID, MemberEditInfoContent content)
        {
            try
            {
                MemberModel memberModel = await this.GetMemberData(memberID).ConfigureAwait(false);
                if (memberModel == null)
                {
                    this.logger.LogWarn("會員編輯資訊結果", $"Result: 搜尋失敗，無會員資料 MemberID: {memberID}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = "無會員資料."
                    };
                }

                string updateInfoHandlerResult = await this.UpdateInfoHandler(content, memberModel).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(updateInfoHandlerResult))
                {
                    this.logger.LogWarn("會員編輯資訊結果", $"Result: 更新失敗({updateInfoHandlerResult}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UpdateFail,
                        Content = updateInfoHandlerResult
                    };
                }

                bool updateResult = await this.memberRepository.Update(memberModel).ConfigureAwait(false);
                if (!updateResult)
                {
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UpdateFail,
                        Content = "更新資料失敗."
                    };
                }

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "更新資料成功."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員編輯資訊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.DenyAccess,
                    Content = "編輯資訊發生錯誤."
                };
            }
        }

        /// <summary>
        /// 會員修改密碼
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> EditPassword(string memberID, MemberEditPasswordContent content)
        {
            try
            {
                #region 驗證資料

                MemberEditPasswordContentValidator memberEditPasswordContentValidator = new MemberEditPasswordContentValidator();
                ValidationResult validationResult = memberEditPasswordContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("會員修改密碼結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                IEnumerable<MemberModel> memberModels = await this.memberRepository.Get(memberID, false).ConfigureAwait(false);
                if (memberModels == null)
                {
                    this.logger.LogFatal("會員修改密碼結果", $"Result: 無會員資料, 須查詢 DB 比對或 JWT 錯誤 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = "無會員資料."
                    };
                }

                MemberModel memberModel = memberModels.FirstOrDefault();
                if (!Utility.DecryptAES(memberModel.Password).Equals(content.CurrentPassword))
                {
                    this.logger.LogWarn("會員修改密碼結果", $"Result: 密碼錯誤 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "密碼錯誤."
                    };
                }

                memberModel.Password = Utility.EncryptAES(content.ConfirmPassword);
                bool updateResult = await this.memberRepository.Update(memberModel).ConfigureAwait(false);
                if (!updateResult)
                {
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UpdateFail,
                        Content = "更新資料失敗."
                    };
                }

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "修改密碼成功."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員請求修改密碼發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "修改密碼發生錯誤."
                };
            }
        }

        /// <summary>
        /// 搜尋會員(模糊比對)
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> FuzzySearch(MemberSearchContent content, string searchMemberID)
        {
            try
            {
                #region 驗證資料

                MemberSearchContentValidator memberSearchContentValidator = new MemberSearchContentValidator();
                ValidationResult validationResult = memberSearchContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("搜尋會員結果(模糊比對)", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                IEnumerable<MemberModel> memberModels = await this.memberRepository.GetByFuzzy(content.SearchKey);
                List<MemberSimpleInfoViewDto> memberSimpleInfoViewDtos = new List<MemberSimpleInfoViewDto>();
                foreach (MemberModel memberModel in memberModels)
                {
                    //// 略過會員本人資料
                    if (memberModel.MemberID.Equals(searchMemberID))
                    {
                        continue;
                    }

                    //// TODO 待檢驗其他會員是否同意被檢閱資料

                    string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-login-{memberModel.MemberID}";
                    MemberSimpleInfoViewDto memberSimpleInfoViewDto = this.mapper.Map<MemberSimpleInfoViewDto>(memberModel);
                    memberSimpleInfoViewDto.OnlineType = await this.redisRepository.IsExist(cacheKey) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
                    memberSimpleInfoViewDtos.Add(memberSimpleInfoViewDto);
                }

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = memberSimpleInfoViewDtos
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋會員發生錯誤(模糊比對)", $"Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.DenyAccess,
                    Content = "搜尋會員發生錯誤."
                };
            }
        }

        /// <summary>
        /// 初始化會員密碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> InitPassword(MemberForgetPasswordContent content)
        {
            try
            {
                #region 驗證資料

                MemberForgetPasswordContentValidator memberForgetPasswordContentValidator = new MemberForgetPasswordContentValidator(true);
                ValidationResult validationResult = memberForgetPasswordContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("初始化會員密碼結果", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 比對驗證碼

                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{VerifierType.ForgetPassword}-{content.Email}";
                string verifierCode = await this.redisRepository.GetCache<string>(cacheKey).ConfigureAwait(false);
                if (!content.VerifierCode.Equals(verifierCode))
                {
                    this.logger.LogWarn("初始化會員密碼結果", $"Result: 驗證碼錯誤, Content: {JsonConvert.SerializeObject(content)} VerifierCode: {verifierCode}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "驗證碼錯誤."
                    };
                }

                #endregion 比對驗證碼

                #region 更新使用者密碼

                MemberModel memberModel = await this.memberRepository.GetByEmail(content.Email).ConfigureAwait(false);
                if (memberModel == null)
                {
                    this.logger.LogWarn("初始化會員密碼結果", $"Result: 無會員資料，須查詢 DB 比對 Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = "無會員資料."
                    };
                }

                string password = Guid.NewGuid().ToString().Substring(0, 8);
                memberModel.Password = Utility.EncryptAES(password);
                bool updateResult = await this.memberRepository.Update(memberModel).ConfigureAwait(false);
                if (!updateResult)
                {
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UpdateFail,
                        Content = "更新資料失敗."
                    };
                }

                #endregion 更新使用者密碼

                #region 發送郵件

                EmailContext emailContext = EmailContext.GetResetPasswordEmailContext(content.Email, password);
                string postData = JsonConvert.SerializeObject(emailContext);
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.SmtpServer.Domain, AppSettingHelper.Appsetting.SmtpServer.SendEmailApi, postData).ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("初始化會員密碼結果", $"Result: 發送郵件失敗({httpResponseMessage.Content}) EmailContext: {JsonConvert.SerializeObject(emailContext)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = "發送郵件失敗."
                    };
                }

                #endregion 發送郵件

                #region 刪除 Redis 驗證碼

                this.redisRepository.DeleteCache(cacheKey);

                #endregion 刪除 Redis 驗證碼

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "初始化密碼成功."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送會員忘記密碼驗證碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "發送驗證碼發生錯誤."
                };
            }
        }

        /// <summary>
        /// 會員手機綁定
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> MobileBind(string memberID, MemberMobileBindContent content)
        {
            try
            {
                #region 驗證資料

                MemberMobileBindContentValidator memberMobileBindContentValidator = new MemberMobileBindContentValidator(true);
                ValidationResult validationResult = memberMobileBindContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("會員手機綁定結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 比對驗證碼

                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{VerifierType.MobileBind}-{content.Mobile}";
                string verifierCode = await this.redisRepository.GetCache<string>(cacheKey).ConfigureAwait(false);
                if (!content.VerifierCode.Equals(verifierCode))
                {
                    this.logger.LogWarn("會員手機綁定結果", $"Result: 驗證碼錯誤, MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} VerifierCode: {verifierCode}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "驗證碼錯誤."
                    };
                }

                #endregion 比對驗證碼

                #region 檢查手機是否已被綁定

                IEnumerable<MemberModel> memberModels = await this.memberRepository.Get(content.Mobile, false).ConfigureAwait(false);
                if (memberModels != null)
                {
                    this.logger.LogWarn("會員手機綁定結果", $"Result: 該手機已被綁定, MemberID: {memberID} BindMemberID:{memberModels.FirstOrDefault().MemberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = "該手機已被綁定."
                    };
                }

                #endregion 檢查手機是否已被綁定

                #region 綁定使用者手機

                memberModels = await this.memberRepository.Get(memberID, false).ConfigureAwait(false);
                if (memberModels == null)
                {
                    this.logger.LogWarn("會員手機綁定結果", $"Result: 無會員資料，須查詢 DB 比對 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = "無會員資料."
                    };
                }

                MemberModel memberModel = memberModels.FirstOrDefault();
                memberModel.Mobile = content.Mobile;
                bool updateResult = await this.memberRepository.Update(memberModel).ConfigureAwait(false);
                if (!updateResult)
                {
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UpdateFail,
                        Content = "更新資料失敗."
                    };
                }

                #endregion 綁定使用者手機

                #region 刪除 Redis 驗證碼

                this.redisRepository.DeleteCache(cacheKey);

                #endregion 刪除 Redis 驗證碼

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "手機綁定成功."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員手機綁定發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "手機綁定發生錯誤."
                };
            }
        }

        /// <summary>
        /// 發送會員忘記密碼驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> SendForgetPasswordVerifierCode(MemberForgetPasswordContent content)
        {
            try
            {
                #region 驗證資料

                MemberForgetPasswordContentValidator memberForgetPasswordContentValidator = new MemberForgetPasswordContentValidator(false);
                ValidationResult validationResult = memberForgetPasswordContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("發送會員忘記密碼驗證碼結果", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 產生驗證碼

                string verifierCode = Guid.NewGuid().ToString().Substring(0, 6);
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{VerifierType.ForgetPassword}-{content.Email}";
                this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(verifierCode), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.VerifierCodeExpirationDate));

                #endregion 產生驗證碼

                #region 發送郵件

                EmailContext emailContext = EmailContext.GetVerifierCodetEmailContextForForgetPassword(content.Email, verifierCode);
                string postData = JsonConvert.SerializeObject(emailContext);
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.SmtpServer.Domain, AppSettingHelper.Appsetting.SmtpServer.SendEmailApi, postData).ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("發送會員忘記密碼驗證碼結果", $"Result: 發送郵件失敗({httpResponseMessage.Content}) EmailContext: {JsonConvert.SerializeObject(emailContext)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = "發送郵件失敗."
                    };
                }

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "已發送驗證碼."
                };

                #endregion 發送郵件
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送會員忘記密碼驗證碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.DenyAccess,
                    Content = "發送驗證碼發生錯誤."
                };
            }
        }

        /// <summary>
        /// 發送會員手機綁定驗證碼
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> SendMobileBindVerifierCode(string email, MemberMobileBindContent content)
        {
            try
            {
                #region 驗證資料

                MemberMobileBindContentValidator memberMobileBindContentValidator = new MemberMobileBindContentValidator(false);
                ValidationResult validationResult = memberMobileBindContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("發送會員手機綁定驗證碼結果", $"Result: 驗證失敗({errorMessgae}) Email: {email} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 檢查手機是否已被綁定

                IEnumerable<MemberModel> memberModels = await this.memberRepository.Get(content.Mobile, false).ConfigureAwait(false);
                if (memberModels != null)
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼結果", $"Result: 該手機已被綁定, Email: {email} BindMemberID:{memberModels.FirstOrDefault().MemberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = "該手機已被綁定."
                    };
                }

                #endregion 檢查手機是否已被綁定

                #region 產生驗證碼

                string verifierCode = Guid.NewGuid().ToString().Substring(0, 6);
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.VerifierCode}-{VerifierType.MobileBind}-{content.Mobile}";
                this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(verifierCode), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.VerifierCodeExpirationDate));

                #endregion 產生驗證碼

                #region 發送郵件

                EmailContext emailContext = EmailContext.GetVerifierCodetEmailContextForMobileBind(email, verifierCode);
                string postData = JsonConvert.SerializeObject(emailContext);
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.SmtpServer.Domain, AppSettingHelper.Appsetting.SmtpServer.SendEmailApi, postData).ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼結果", $"Result: 發送郵件失敗({httpResponseMessage.Content}) EmailContext: {JsonConvert.SerializeObject(emailContext)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = "發送郵件失敗."
                    };
                }

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "已發送驗證碼."
                };

                #endregion 發送郵件
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送會員手機綁定驗證碼發生錯誤", $"Email: {email} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "發送驗證碼發生錯誤."
                };
            }
        }

        /// <summary>
        /// 搜尋會員(嚴格比對)
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> StrictSearch(MemberSearchContent content, string searchMemberID = null)
        {
            try
            {
                #region 驗證資料

                MemberSearchContentValidator memberSearchContentValidator = new MemberSearchContentValidator();
                ValidationResult validationResult = memberSearchContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("搜尋會員結果(嚴格比對)", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                MemberModel memberModel = await this.GetMemberData(content.SearchKey).ConfigureAwait(false);
                if (memberModel == null)
                {
                    this.logger.LogWarn("搜尋會員結果(嚴格比對)", $"Result: 搜尋失敗，無會員資料 Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", null);
                    return new ResponseResultDto()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = null
                    };
                }

                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-login-{memberModel.MemberID}";
                Task<bool> onlineResult = this.redisRepository.IsExist(cacheKey);

                #region 取得會員本身資料

                if (string.IsNullOrEmpty(searchMemberID))
                {
                    MemberDetailInfoViewDto memberDetailInfoViewDto = this.mapper.Map<MemberDetailInfoViewDto>(memberModel);
                    memberDetailInfoViewDto.OnlineType = await onlineResult.ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
                    return new ResponseResultDto()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = memberDetailInfoViewDto //// 會員本身資料以詳細顯示
                    };
                }

                #endregion 取得會員本身資料

                #region 取得其他會員資料

                if (memberModel.MemberID.Equals(searchMemberID))
                {
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = "無法搜尋會員本人資料."
                    };
                }

                //// TODO 待檢驗其他會員是否同意被檢閱資料
                MemberSimpleInfoViewDto memberSimpleInfoViewDto = this.mapper.Map<MemberSimpleInfoViewDto>(memberModel);
                memberSimpleInfoViewDto.OnlineType = await onlineResult.ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = memberSimpleInfoViewDto //// 其他會員資料以簡易顯示
                };

                #endregion 取得其他會員資料
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋會員發生錯誤(嚴格比對)", $"Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.DenyAccess,
                    Content = "搜尋會員發生錯誤."
                };
            }
        }

        #endregion 會員資料
    }
}