using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Ride;
using DataInfo.Core.Models.Dto.Common;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Member.Request;
using DataInfo.Core.Models.Dto.Member.Request.Data;
using DataInfo.Core.Models.Dto.Member.Response;
using DataInfo.Core.Models.Dto.Member.View;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Server;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Interfaces.Server;
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
        /// rideRepository
        /// </summary>
        private readonly IRideRepository rideRepository;

        /// <summary>
        /// serverService
        /// </summary>
        private readonly IServerService serverService;

        /// <summary>
        /// uploadService
        /// </summary>
        private readonly IUploadService uploadService;

        /// <summary>
        /// verifyCodeService
        /// </summary>
        private readonly IVerifyCodeService verifyCodeService;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="jwtService">jwtService</param>
        /// <param name="uploadService">uploadService</param>
        /// <param name="verifyCodeService">verifyCodeService</param>
        /// <param name="serverService">serverService</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="rideRepository">rideRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public MemberService(IMapper mapper, IJwtService jwtService, IUploadService uploadService, IVerifyCodeService verifyCodeService, IServerService serverService, IMemberRepository memberRepository, IRideRepository rideRepository, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.jwtService = jwtService;
            this.uploadService = uploadService;
            this.verifyCodeService = verifyCodeService;
            this.serverService = serverService;
            this.memberRepository = memberRepository;
            this.rideRepository = rideRepository;
            this.redisRepository = redisRepository;
        }

        #region 註冊 \ 登入 \ 登出 \ 保持在線

        /// <summary>
        /// 生產 Jwt Token
        /// </summary>
        /// <param name="memberDao">memberDao</param>
        /// <returns>string</returns>
        private string GenerateJwtToken(MemberDao memberDao)
        {
            JwtClaims jwtClaims = new JwtClaims()
            {
                MemberID = memberDao.MemberID,
                Email = memberDao.Email,
                Nickname = memberDao.Nickname,
                Avatar = memberDao.Avatar,
                FrontCover = memberDao.FrontCover,
                Mobile = memberDao.Mobile
            };
            return this.jwtService.GenerateToken(jwtClaims);
        }

        /// <summary>
        /// 更新會員最新登入時間
        /// </summary>
        /// <param name="memberDao">memberDao</param>
        private void UpdateLastLoginDate(MemberDao memberDao)
        {
            try
            {
                this.logger.LogInfo("更新會員最新登入時間", $"MemberID: {memberDao.MemberID}", null);
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{AppSettingHelper.Appsetting.Redis.Flag.LastLogin}-{memberDao.MemberID}";
                this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(DateTime.UtcNow), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.KeepOnlineTime));
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新會員最新登入時間發生錯誤", $"MemberID: {memberDao.MemberID}", ex);
            }
        }

        /// <summary>
        /// 會員保持在線
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> KeepOnline(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{AppSettingHelper.Appsetting.Redis.Flag.LastLogin}-{memberID}";
                bool result = await this.redisRepository.UpdateCacheExpire(cacheKey, TimeSpan.FromMinutes(AppSettingHelper.Appsetting.KeepOnlineTime)).ConfigureAwait(false);
                if (result)
                {
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = MessageHelper.Message.ResponseMessage.Update.Success
                    };
                }

                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UpdateFail,
                    Content = MessageHelper.Message.ResponseMessage.Update.Fail
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員保持在線發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 會員登入(一般登入)
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Login(MemberLoginContent content)
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
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 發送【使用者登入】指令至後端

                MemberLoginRequest request = new MemberLoginRequest()
                {
                    Email = content.Email,
                    Password = content.Password,
                };

                CommandData<MemberLoginResponse> response = await this.serverService.DoAction<MemberLoginResponse>((int)UserCommandIDType.UserLogin, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("會員登入結果(一般登入)", $"Result: {response.Data.Result} Email: {content.Email} Password: {content.Password}", null);
                switch (response.Data.Result)
                {
                    case (int)UserLoginResultType.Success:
                        MemberDao memberDao = (await this.memberRepository.Get(response.Data.MemberID).ConfigureAwait(false));

                        #region 更新最新登入時間

                        this.UpdateLastLoginDate(memberDao);

                        #endregion 更新最新登入時間

                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = new MemberLoginView() { Token = this.GenerateJwtToken(memberDao) }
                        };

                    case (int)UserLoginResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.DenyAccess,
                            Content = MessageHelper.Message.ResponseMessage.Login.Fail
                        };

                    case (int)UserLoginResultType.EmailError:
                    case (int)UserLoginResultType.PasswordError:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.InputError,
                            Content = MessageHelper.Message.ResponseMessage.Login.Fail
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Login.Fail
                        };
                }

                #endregion 發送【使用者登入】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員登入發生錯誤(一般登入)", $"Email: {content.Email} Password: {content.Password}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Login.Error
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
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Register(MemberRegisterContent content, bool isValidatePassword, string fbToken, string googleToken)
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
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 發送【使用者註冊】指令至後端

                MemberRegisterRequest request = new MemberRegisterRequest()
                {
                    Email = content.Email,
                    Password = content.Password,
                    CheckPassword = content.ConfirmPassword,
                    FBToken = fbToken,
                    GoogleToken = googleToken,
                    RegisterSource = string.IsNullOrEmpty(fbToken) ? string.IsNullOrEmpty(googleToken) ? (int)RegisterSourceType.Normal : (int)RegisterSourceType.Google : (int)RegisterSourceType.FB
                };

                CommandData<MemberRegisterResponse> response = await this.serverService.DoAction<MemberRegisterResponse>((int)UserCommandIDType.UserRegistered, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("會員註冊結果", $"Result: {response.Data.Result} Email: {content.Email} Password: {content.Password} ConfirmPassword: {content.ConfirmPassword} IsValidatePassword: {isValidatePassword} FbToken: {fbToken} GoogleToken: {googleToken}", null);
                switch (response.Data.Result)
                {
                    case (int)UserRegisteredResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Register.Success
                        };

                    case (int)UserRegisteredResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.CreateFail,
                            Content = MessageHelper.Message.ResponseMessage.Register.Fail
                        };

                    case (int)UserRegisteredResultType.EmailError:
                    case (int)UserRegisteredResultType.PasswordError:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.InputError,
                            Content = MessageHelper.Message.ResponseMessage.Register.Fail
                        };

                    case (int)UserRegisteredResultType.Repeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.Existed,
                            Content = MessageHelper.Message.ResponseMessage.Register.EmailExist
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Register.Fail
                        };
                }

                #endregion 發送【使用者註冊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員註冊發生錯誤", $"Email: {content.Email} Password: {content.Password} ConfirmPassword: {content.ConfirmPassword} IsValidatePassword: {isValidatePassword} FbToken: {fbToken} GoogleToken: {googleToken}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Register.Error
                };
            }
        }

        /// <summary>
        /// 會員登入(重新登入)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Relogin(string memberID)
        {
            try
            {
                #region 取得會員資料

                MemberDao memberDao = (await this.memberRepository.Get(memberID, false, null).ConfigureAwait(false)).FirstOrDefault();
                if (memberDao == null)
                {
                    this.logger.LogWarn("會員登入結果(重新登入)", $"Result: 無會員資料，須查詢 DB 比對 MemberID: {memberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = MessageHelper.Message.ResponseMessage.Member.MemberNotExist
                    };
                }

                #endregion 取得會員資料

                #region 更新最新登入時間

                this.UpdateLastLoginDate(memberDao);

                #endregion 更新最新登入時間

                this.logger.LogInfo("會員登入成功(重新登入)", $"MemberID: {memberID}", null);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = new MemberLoginView() { Token = this.GenerateJwtToken(memberDao) }
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員登入發生錯誤(重新登入)", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Login.Error
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
        /// 會員資料更新處理
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>Tuple(string, MemberEditInfoRequest)</returns>
        private async Task<Tuple<string, MemberEditInfoRequest>> UpdateInfoHandler(string memberID, MemberEditInfoContent content)
        {
            MemberUpdateInfoData memberUpdateInfoData = new MemberUpdateInfoData();
            if (!string.IsNullOrEmpty(content.Avatar) || !string.IsNullOrEmpty(content.FrontCover) || !string.IsNullOrEmpty(content.Photo))
            {
                List<string> imgBase64s = new List<string>() { content.Avatar, content.FrontCover, content.Photo };
                IEnumerable<string> imgUris = await this.uploadService.UploadMemberImages(imgBase64s, true).ConfigureAwait(false);
                if (imgUris == null || !imgUris.Any())
                {
                    return Tuple.Create<string, MemberEditInfoRequest>(MessageHelper.Message.ResponseMessage.Upload.PhotoFail, null);
                }

                if (!string.IsNullOrEmpty(content.Avatar))
                {
                    string avatar = imgUris.ElementAt(0);
                    if (string.IsNullOrEmpty(avatar))
                    {
                        return Tuple.Create<string, MemberEditInfoRequest>(MessageHelper.Message.ResponseMessage.Upload.AvatarFail, null);
                    }

                    memberUpdateInfoData.Avatar = avatar;
                }

                if (!string.IsNullOrEmpty(content.FrontCover))
                {
                    string frontCover = imgUris.ElementAt(1);
                    if (string.IsNullOrEmpty(frontCover))
                    {
                        return Tuple.Create<string, MemberEditInfoRequest>(MessageHelper.Message.ResponseMessage.Upload.FrontCoverFail, null);
                    }

                    memberUpdateInfoData.FrontCover = frontCover;
                }

                if (!string.IsNullOrEmpty(content.Photo))
                {
                    string photo = imgUris.ElementAt(2);
                    if (string.IsNullOrEmpty(photo))
                    {
                        return Tuple.Create<string, MemberEditInfoRequest>(MessageHelper.Message.ResponseMessage.Upload.HomePhotoFail, null);
                    }

                    memberUpdateInfoData.Photo = photo;
                }
            }

            if (content.Birthday.HasValue)
            {
                memberUpdateInfoData.Birthday = content.Birthday.Value.ToString("yyyy/MM/dd HH:mm:ss");
            }

            if (content.BodyHeight > 0)
            {
                memberUpdateInfoData.BodyHeight = content.BodyHeight;
            }

            if (content.BodyWeight > 0)
            {
                memberUpdateInfoData.BodyWeight = content.BodyWeight;
            }

            if (content.Gender > (int)GenderType.None)
            {
                memberUpdateInfoData.Gender = content.Gender;
            }

            if (!string.IsNullOrEmpty(content.Nickname))
            {
                memberUpdateInfoData.NickName = content.Nickname;
            }

            MemberEditInfoRequest memberEditInfoRequest = new MemberEditInfoRequest() { MemberID = memberID, UpdateData = memberUpdateInfoData };
            return Tuple.Create(string.Empty, memberEditInfoRequest);
        }

        /// <summary>
        /// 會員編輯資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> EditInfo(string memberID, MemberEditInfoContent content)
        {
            try
            {
                #region 處理更新資料

                Tuple<string, MemberEditInfoRequest> updateInfoHandlerResult = await this.UpdateInfoHandler(memberID, content).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(updateInfoHandlerResult.Item1))
                {
                    this.logger.LogWarn("會員編輯資訊結果", $"Result: 更新失敗({updateInfoHandlerResult.Item1}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.UpdateFail,
                        Content = updateInfoHandlerResult.Item1
                    };
                }

                MemberEditInfoRequest request = updateInfoHandlerResult.Item2;

                #endregion 處理更新資料

                #region 發送【更新使用者資訊】指令至後端

                CommandData<MemberEditInfoResponse> response = await this.serverService.DoAction<MemberEditInfoResponse>((int)UserCommandIDType.UpdateUserInfo, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("會員編輯資訊結果", $"Result: {response.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateUserInfoResultType.Success:
                        MemberDao memberDao = (await this.memberRepository.Get(memberID, false, null).ConfigureAwait(false)).FirstOrDefault();
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = this.GenerateJwtToken(memberDao)
                        };

                    case (int)UpdateUserInfoResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };
                }

                #endregion 發送【更新使用者資訊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員編輯資訊發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 搜尋會員(模糊比對)
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> FuzzySearch(MemberSearchContent content, string searchMemberID)
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
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                string[] ignoreMemberIds = new string[] { searchMemberID };
                IEnumerable<MemberDao> memberDaos = await this.memberRepository.Get(content.SearchKey, true, ignoreMemberIds).ConfigureAwait(false);
                IEnumerable<MemberSimpleInfoView> memberSimpleInfoViews = await this.TransformMemberSimpleInfoView(memberDaos).ConfigureAwait(false);

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = memberSimpleInfoViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋會員發生錯誤(模糊比對)", $"Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 取得會員名片資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetCardInfo(string memberID, string searchMemberID = null)
        {
            try
            {
                #region 驗證資料

                if (string.IsNullOrEmpty(memberID))
                {
                    this.logger.LogWarn("取得會員名片資訊結果", $"Result: 驗證失敗，會員編號無效 MemberID: {memberID} SearchMemberID: {searchMemberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty
                    };
                }

                #endregion 驗證資料

                MemberDao memberDao = await this.memberRepository.Get(memberID).ConfigureAwait(false);
                if (memberDao == null)
                {
                    this.logger.LogWarn("取得會員名片資訊結果", $"Result: 無會員資料 MemberID: {memberID} SearchMemberID: {searchMemberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = MessageHelper.Message.ResponseMessage.Member.MemberNotExist
                    };
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = this.mapper.Map<MemberCardInfoView>(memberDao)
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員名片資訊發生錯誤", $"MemberID: {memberID} SearchMemberID: {searchMemberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 取得首頁資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> HomeInfo(string memberID)
        {
            try
            {
                MemberDao memberDao = (await this.memberRepository.Get(memberID, false, null).ConfigureAwait(false)).FirstOrDefault();
                RideDistanceDao rideDistanceDao = await this.rideRepository.GetTotalDistance(memberID).ConfigureAwait(false);
                MemberHomeInfoView memberHomeInfoView = this.mapper.Map<MemberHomeInfoView>(memberDao);
                if (rideDistanceDao != null)
                {
                    memberHomeInfoView.TotalDistance = rideDistanceDao.TotalDistance;
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = memberHomeInfoView
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得首頁資訊發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 會員手機綁定
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> MobileBind(string memberID, MemberMobileBindContent content)
        {
            try
            {
                #region 驗證資料

                MemberMobileBindContentValidator memberMobileBindContentValidator = new MemberMobileBindContentValidator();
                ValidationResult validationResult = memberMobileBindContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("會員手機綁定結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 比對驗證碼

                ResponseResult validateVerifyCodeResult = await this.verifyCodeService.Validate(content.VerifierCode, false).ConfigureAwait(false);
                if (!validateVerifyCodeResult.Result)
                {
                    this.logger.LogWarn("會員手機綁定結果", $"Result: 驗證碼錯誤, MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return validateVerifyCodeResult;
                }

                #endregion 比對驗證碼

                #region 檢查手機是否已被綁定

                MemberDao memberDao = (await this.memberRepository.Get(content.Mobile, false, null).ConfigureAwait(false)).FirstOrDefault();
                if (memberDao != null)
                {
                    this.logger.LogWarn("會員手機綁定結果", $"Result: 該手機已被綁定 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} BindMemberID:{memberDao.MemberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Member.MobileBind
                    };
                }

                memberDao = (await this.memberRepository.Get(memberID, false, null).ConfigureAwait(false)).FirstOrDefault();
                if (!string.IsNullOrEmpty(memberDao.Mobile))
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼結果", $"Result: 會員已綁定手機 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} BindMobile:{memberDao.Mobile}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Member.MemberHasBindMobile
                    };
                }

                #endregion 檢查手機是否已被綁定

                #region 發送【更新使用者資訊】指令至後端

                memberDao.Mobile = content.Mobile;
                MemberEditInfoRequest request = new MemberEditInfoRequest()
                {
                    MemberID = memberDao.MemberID,
                    UpdateData = new MemberUpdateInfoData()
                    {
                        Mobile = content.Mobile
                    }
                };
                CommandData<MemberEditInfoResponse> response = await this.serverService.DoAction<MemberEditInfoResponse>((int)UserCommandIDType.UpdateUserInfo, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("會員手機綁定結果", $"Result: {response.Data.Result} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateUserInfoResultType.Success:

                        #region 刪除 Redis 驗證碼

                        this.verifyCodeService.Delete(content.VerifierCode);

                        #endregion 刪除 Redis 驗證碼

                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = this.GenerateJwtToken(memberDao)
                        };

                    case (int)UpdateUserInfoResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };
                }

                #endregion 發送【更新使用者資訊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員手機綁定發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 重置會員密碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> ResetPassword(MemberForgetPasswordContent content)
        {
            try
            {
                #region 驗證資料

                MemberForgetPasswordContentValidator memberForgetPasswordContentValidator = new MemberForgetPasswordContentValidator();
                ValidationResult validationResult = memberForgetPasswordContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("重置會員密碼結果", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 比對驗證碼

                ResponseResult validateVerifyCodeResult = await this.verifyCodeService.Validate(content.VerifierCode, false).ConfigureAwait(false);
                if (!validateVerifyCodeResult.Result)
                {
                    this.logger.LogWarn("重置會員密碼結果", $"Result: 驗證碼驗證失敗, ResultCode: {validateVerifyCodeResult.ResultCode} Content: {JsonConvert.SerializeObject(content)}", null);
                    return validateVerifyCodeResult;
                }

                #endregion 比對驗證碼

                #region 取得會員資料

                MemberDao memberDao = (await this.memberRepository.Get(content.Email, false, null).ConfigureAwait(false)).FirstOrDefault();
                if (memberDao == null)
                {
                    this.logger.LogWarn("重置會員密碼結果", $"Result: 查無以此信箱註冊的會員 Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = MessageHelper.Message.ResponseMessage.Member.EmailNotExist
                    };
                }

                #endregion 取得會員資料

                #region 更新密碼

                ResponseResult responseResult = await this.UpdatePassword(memberDao.MemberID, new MemberUpdatePasswordContent()
                {
                    NewPassword = content.Password,
                    ConfirmPassword = content.ConfirmPassword
                }, true).ConfigureAwait(false);

                #endregion 更新密碼

                #region 刪除 Redis 驗證碼

                if (responseResult.Result)
                {
                    this.verifyCodeService.Delete(content.VerifierCode);
                }

                #endregion 刪除 Redis 驗證碼

                return responseResult;
            }
            catch (Exception ex)
            {
                this.logger.LogError("重置會員密碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        /// <summary>
        /// 發送會員忘記密碼驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> SendForgetPasswordVerifierCode(MemberRequestForgetPasswordContent content)
        {
            try
            {
                #region 驗證資料

                MemberRequestForgetPasswordContentValidator memberRequestForgetPasswordContentValidator = new MemberRequestForgetPasswordContentValidator();
                ValidationResult validationResult = memberRequestForgetPasswordContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("發送會員忘記密碼驗證碼結果", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                MemberDao memberDao = (await this.memberRepository.Get(content.Email, false, null).ConfigureAwait(false)).FirstOrDefault();
                if (memberDao == null)
                {
                    this.logger.LogWarn("發送會員忘記密碼驗證碼結果", $"Result: 查無以此信箱註冊的會員 Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = MessageHelper.Message.ResponseMessage.Member.EmailNotExist
                    };
                }

                #endregion 驗證資料

                #region 產生驗證碼

                string verifierCode = await this.verifyCodeService.Generate().ConfigureAwait(false);

                #endregion 產生驗證碼

                #region 發送驗證碼

                EmailContext emailContext = EmailContext.GetVerifierCodetEmailContextForForgetPassword(content.Email, verifierCode);
                string postData = JsonConvert.SerializeObject(emailContext);
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.SmtpServer.Domain, AppSettingHelper.Appsetting.SmtpServer.SendEmailApi, postData).ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("發送會員忘記密碼驗證碼結果", $"Result: 發送郵件失敗({httpResponseMessage.Content}) EmailContext: {JsonConvert.SerializeObject(emailContext)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Smtp.SendEmailFail
                    };
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = MessageHelper.Message.ResponseMessage.VerifyCode.SendVerifyCodeSuccess
                };

                #endregion 發送驗證碼
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送會員忘記密碼驗證碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.VerifyCode.SendVerifyCodeError
                };
            }
        }

        /// <summary>
        /// 發送會員手機綁定驗證碼
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> SendMobileBindVerifierCode(string memberID, MemberRequestMobileBindContent content)
        {
            try
            {
                #region 驗證資料

                MemberRequestMobileBindContentValidator memberRequestMobileBindContentValidator = new MemberRequestMobileBindContentValidator();
                ValidationResult validationResult = memberRequestMobileBindContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("發送會員手機綁定驗證碼結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 檢查手機是否已被綁定

                MemberDao memberDao = (await this.memberRepository.Get(content.Mobile, false, null).ConfigureAwait(false)).FirstOrDefault();
                if (memberDao != null)
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼結果", $"Result: 該手機已被綁定 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} BindMemberID:{memberDao.MemberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Member.MobileBind
                    };
                }

                memberDao = (await this.memberRepository.Get(memberID, false, null).ConfigureAwait(false)).FirstOrDefault();
                if (!string.IsNullOrEmpty(memberDao.Mobile))
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼結果", $"Result: 會員已綁定手機 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} BindMobile:{memberDao.Mobile}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.Member.MemberHasBindMobile
                    };
                }

                #endregion 檢查手機是否已被綁定

                #region 產生驗證碼

                string verifierCode = await this.verifyCodeService.Generate().ConfigureAwait(false);

                #endregion 產生驗證碼

                #region 發送驗證碼

                EmailContext emailContext = EmailContext.GetVerifierCodetEmailContextForMobileBind(memberDao.Email, verifierCode);
                string postData = JsonConvert.SerializeObject(emailContext);
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.SmtpServer.Domain, AppSettingHelper.Appsetting.SmtpServer.SendEmailApi, postData).ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼結果", $"Result: 發送郵件失敗({httpResponseMessage.Content}) EmailContext: {JsonConvert.SerializeObject(emailContext)}  MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.DenyAccess,
                        Content = MessageHelper.Message.ResponseMessage.VerifyCode.SendVerifyCodeFail
                    };
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = MessageHelper.Message.ResponseMessage.VerifyCode.SendVerifyCodeSuccess
                };

                #endregion 發送驗證碼
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送會員手機綁定驗證碼發生錯誤", $" MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.VerifyCode.SendVerifyCodeError
                };
            }
        }

        /// <summary>
        /// 搜尋會員(嚴格比對)
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> StrictSearch(MemberSearchContent content, string searchMemberID = null)
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
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 取得指定會員資料

                string[] ignoreMemberIDs = string.IsNullOrEmpty(searchMemberID) ? null : new string[] { searchMemberID };
                IEnumerable<MemberDao> memberDaos = await this.memberRepository.Get(content.SearchKey, false, ignoreMemberIDs).ConfigureAwait(false);
                if (string.IsNullOrEmpty(searchMemberID))
                {
                    //// 會員本身資料
                    IEnumerable<MemberDetailInfoView> memberDetailInfoViews = await this.TransformMemberDetailInfoView(memberDaos).ConfigureAwait(false);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = memberDetailInfoViews.FirstOrDefault() //// 會員本身資料以詳細顯示
                    };
                }
                else
                {
                    //// 其他會員資料
                    IEnumerable<MemberSimpleInfoView> memberSimpleInfoViews = await this.TransformMemberSimpleInfoView(memberDaos).ConfigureAwait(false);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = (int)ResponseResultType.Success,
                        Content = memberSimpleInfoViews.FirstOrDefault() //// 其他會員資料以簡易顯示
                    };
                }

                #endregion 取得指定會員資料
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋會員發生錯誤(嚴格比對)", $"Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Get.Error
                };
            }
        }

        /// <summary>
        /// 轉換為會員詳細資訊可視資料
        /// </summary>
        /// <param name="memberDaos">memberDaos</param>
        /// <returns>MemberDetailInfoViews</returns>
        public async Task<IEnumerable<MemberDetailInfoView>> TransformMemberDetailInfoView(IEnumerable<MemberDao> memberDaos)
        {
            List<MemberDetailInfoView> memberDetailInfoViews = new List<MemberDetailInfoView>();
            if (memberDaos != null)
            {
                foreach (MemberDao memberDao in memberDaos)
                {
                    //// TODO 待檢驗會員是否同意被檢閱資料

                    string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{AppSettingHelper.Appsetting.Redis.Flag.LastLogin}-{memberDao.MemberID}";
                    MemberDetailInfoView memberDetailInfoView = this.mapper.Map<MemberDetailInfoView>(memberDao);
                    memberDetailInfoView.OnlineType = await this.redisRepository.IsExist(cacheKey).ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
                    memberDetailInfoViews.Add(memberDetailInfoView);
                }
            }

            return memberDetailInfoViews;
        }

        /// <summary>
        /// 轉換為會員簡易資訊可視資料
        /// </summary>
        /// <param name="memberDaos">memberDaos</param>
        /// <returns>MemberSimpleInfoViews</returns>
        public async Task<IEnumerable<MemberSimpleInfoView>> TransformMemberSimpleInfoView(IEnumerable<MemberDao> memberDaos)
        {
            List<MemberSimpleInfoView> memberSimpleInfoViews = new List<MemberSimpleInfoView>();
            if (memberDaos.Any())
            {
                foreach (MemberDao memberDao in memberDaos)
                {
                    //// TODO 待檢驗會員是否同意被檢閱資料

                    string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{AppSettingHelper.Appsetting.Redis.Flag.LastLogin}-{memberDao.MemberID}";
                    MemberSimpleInfoView memberSimpleInfoView = this.mapper.Map<MemberSimpleInfoView>(memberDao);
                    memberSimpleInfoView.OnlineType = await this.redisRepository.IsExist(cacheKey).ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
                    memberSimpleInfoViews.Add(memberSimpleInfoView);
                }
            }

            return memberSimpleInfoViews;
        }

        /// <summary>
        /// 會員更新密碼
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <param name="isIgnoreOldPassword">isIgnoreOldPassword</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdatePassword(string memberID, MemberUpdatePasswordContent content, bool isIgnoreOldPassword)
        {
            try
            {
                #region 驗證資料

                MemberUpdatePasswordContentValidator memberEditPasswordContentValidator = new MemberUpdatePasswordContentValidator(isIgnoreOldPassword);
                ValidationResult validationResult = memberEditPasswordContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("會員更新密碼結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} isIgnoreOldPassword: {isIgnoreOldPassword}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 發送【更新密碼】指令至後端

                MemberUpdatePasswordRequest request = new MemberUpdatePasswordRequest()
                {
                    MemberID = memberID,
                    Password = content.Password,
                    NewPassword = content.NewPassword
                };
                CommandData<MemberEditInfoResponse> response = await this.serverService.DoAction<MemberEditInfoResponse>((int)UserCommandIDType.UpdatePassword, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("會員更新密碼結果", $"Result: {response.Data.Result} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdatePasswordResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Update.Success
                        };

                    case (int)UpdatePasswordResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UpdateFail,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };

                    case (int)UpdatePasswordResultType.OldPasswordError:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.InputError,
                            Content = MessageHelper.Message.ResponseMessage.Member.PasswordFail
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Update.Fail
                        };
                }

                #endregion 發送【更新密碼】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員更新密碼發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Update.Error
                };
            }
        }

        #endregion 會員資料
    }
}