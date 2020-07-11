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
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Repository.Interfaces.Ride;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Interfaces.Server;
using Microsoft.AspNetCore.Http;
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
        /// 更新會員最新登入時間
        /// </summary>
        /// <param name="memberID">memberID</param>
        private void UpdateLastLoginDate(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.LastLogin}";
                this.redisRepository.SetCache(cacheKey, DateTime.UtcNow.ToString(), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.KeepOnlineTime));
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新會員最新登入時間發生錯誤", $"MemberID: {memberID}", ex);
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
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.LastLogin}";
                bool result = await this.redisRepository.UpdateCacheExpire(cacheKey, TimeSpan.FromMinutes(AppSettingHelper.Appsetting.KeepOnlineTime)).ConfigureAwait(false);
                if (!result)
                {
                    this.logger.LogWarn("會員保持在線失敗，無法更新 Redis", $"CacheKey: {cacheKey}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status500InternalServerError,
                        ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                    };
                }
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員保持在線發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 會員登入
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Login(MemberLoginContent content)
        {
            try
            {
                #region 發送【使用者登入】指令至後端

                MemberLoginRequest request = this.mapper.Map<MemberLoginRequest>(content);
                CommandData<MemberLoginResponse> response = await this.serverService.DoAction<MemberLoginResponse>((int)UserCommandIDType.UserLogin, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("會員登入結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);
                switch (response.Data.Result)
                {
                    case (int)UserLoginResultType.Success:
                        MemberDao memberDao = await this.memberRepository.Get(response.Data.MemberID, MemberSearchType.MemberID).ConfigureAwait(false);
                        if (memberDao == null)
                        {
                            this.logger.LogError("會員登入失敗，無會員資料但 Server 允許登入了", $"Content: {JsonConvert.SerializeObject(content)}", null);
                            return new ResponseResult()
                            {
                                Result = false,
                                ResultCode = StatusCodes.Status502BadGateway,
                                ResultMessage = ResponseErrorMessageType.LoginFail.ToString()
                            };
                        }

                        #region 更新最新登入時間

                        this.UpdateLastLoginDate(memberDao.MemberID);

                        #endregion 更新最新登入時間

                        JwtClaims jwtClaims = this.mapper.Map<JwtClaims>(memberDao);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            Content = new MemberLoginView() { Token = this.jwtService.GenerateToken(jwtClaims) }
                        };

                    case (int)UserLoginResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.LoginFail.ToString()
                        };

                    case (int)UserLoginResultType.EmailError:
                    case (int)UserLoginResultType.PasswordError:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.EmailOrPasswordNotMatch.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【使用者登入】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員登入發生錯誤(一般登入)", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Register(MemberRegisterContent content, string fbToken, string googleToken)
        {
            try
            {
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
                this.logger.LogInfo("會員註冊結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);
                switch (response.Data.Result)
                {
                    case (int)UserRegisteredResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK
                        };

                    case (int)UserRegisteredResultType.Fail:
                    case (int)UserRegisteredResultType.EmailError:
                    case (int)UserRegisteredResultType.PasswordError:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.RegisterFail.ToString()
                        };

                    case (int)UserRegisteredResultType.Repeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.EmailRepeat.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【使用者註冊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員註冊發生錯誤", $"Content: {JsonConvert.SerializeObject(content)} FbToken: {fbToken} GoogleToken: {googleToken}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 會員重新登入
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Relogin(string memberID)
        {
            try
            {
                #region 取得會員資料

                MemberDao memberDao = await this.memberRepository.Get(memberID, MemberSearchType.MemberID).ConfigureAwait(false);
                if (memberDao == null)
                {
                    this.logger.LogWarn("會員重新登入失敗，無會員資料", $"MemberID: {memberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.LoginFail.ToString()
                    };
                }

                #endregion 取得會員資料

                #region 更新最新登入時間

                this.UpdateLastLoginDate(memberDao.MemberID);

                #endregion 更新最新登入時間

                JwtClaims jwtClaims = this.mapper.Map<JwtClaims>(memberDao);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = new MemberLoginView() { Token = this.jwtService.GenerateToken(jwtClaims) }
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員重新登入發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        #endregion 註冊 \ 登入 \ 登出 \ 保持在線

        #region 會員資料

        /// <summary>
        /// 會員資料更新處理
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>Tuple(string, MemberEditInfoRequest)</returns>
        private async Task<Tuple<string, MemberEditInfoRequest>> UpdateInfoHandler(MemberEditInfoContent content, string memberID)
        {
            MemberUpdateInfoData memberUpdateInfoData = new MemberUpdateInfoData();
            if (!string.IsNullOrEmpty(content.Avatar) || !string.IsNullOrEmpty(content.FrontCover) || !string.IsNullOrEmpty(content.Photo))
            {
                List<string> imgBase64s = new List<string>() { content.Avatar, content.FrontCover, content.Photo };
                IEnumerable<string> imgUris = await this.uploadService.UploadMemberImages(imgBase64s, true).ConfigureAwait(false);
                if (imgUris == null || !imgUris.Any())
                {
                    return Tuple.Create<string, MemberEditInfoRequest>(ResponseErrorMessageType.UploadPhotoFail.ToString(), null);
                }

                if (!string.IsNullOrEmpty(content.Avatar))
                {
                    string avatar = imgUris.ElementAt(0);
                    if (string.IsNullOrEmpty(avatar))
                    {
                        return Tuple.Create<string, MemberEditInfoRequest>(ResponseErrorMessageType.UploadAvatarFail.ToString(), null);
                    }

                    memberUpdateInfoData.Avatar = avatar;
                }

                if (!string.IsNullOrEmpty(content.FrontCover))
                {
                    string frontCover = imgUris.ElementAt(1);
                    if (string.IsNullOrEmpty(frontCover))
                    {
                        return Tuple.Create<string, MemberEditInfoRequest>(ResponseErrorMessageType.UploadFrontCoverFail.ToString(), null);
                    }

                    memberUpdateInfoData.FrontCover = frontCover;
                }

                if (!string.IsNullOrEmpty(content.Photo))
                {
                    string photo = imgUris.ElementAt(2);
                    if (string.IsNullOrEmpty(photo))
                    {
                        return Tuple.Create<string, MemberEditInfoRequest>(ResponseErrorMessageType.UploadHomePhotoFail.ToString(), null);
                    }

                    memberUpdateInfoData.Photo = photo;
                }
            }

            if (!string.IsNullOrEmpty(content.Birthday))
            {
                if (!DateTime.TryParse(content.Birthday, out DateTime birthday))
                {
                    return Tuple.Create<string, MemberEditInfoRequest>(ResponseErrorMessageType.BirthdayFormatError.ToString(), null);
                }

                memberUpdateInfoData.Birthday = birthday.ToString("yyyy-MM-dd");
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

            if (!string.IsNullOrEmpty(content.Nickname.Trim()))
            {
                if (content.Nickname.Length > AppSettingHelper.Appsetting.Rule.NicknameLength)
                {
                    return Tuple.Create<string, MemberEditInfoRequest>(ResponseErrorMessageType.NicknameFormatError.ToString(), null);
                }

                memberUpdateInfoData.NickName = content.Nickname;
            }

            MemberEditInfoRequest memberEditInfoRequest = new MemberEditInfoRequest() { MemberID = memberID, UpdateData = memberUpdateInfoData };
            return Tuple.Create(string.Empty, memberEditInfoRequest);
        }

        /// <summary>
        /// 會員編輯資訊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> EditInfo(MemberEditInfoContent content, string memberID)
        {
            try
            {
                #region 處理更新資料

                Tuple<string, MemberEditInfoRequest> updateInfoHandlerResult = await this.UpdateInfoHandler(content, memberID).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(updateInfoHandlerResult.Item1))
                {
                    this.logger.LogWarn("會員編輯資訊更新失敗，資料驗證錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = updateInfoHandlerResult.Item1
                    };
                }

                MemberEditInfoRequest request = updateInfoHandlerResult.Item2;

                #endregion 處理更新資料

                #region 發送【更新使用者資訊】指令至後端

                CommandData<MemberEditInfoResponse> response = await this.serverService.DoAction<MemberEditInfoResponse>((int)UserCommandIDType.UpdateUserInfo, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("會員編輯資訊結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateUserInfoResultType.Success:
                        MemberDao memberDao = await this.memberRepository.Get(memberID, MemberSearchType.MemberID).ConfigureAwait(false);
                        //// TODO 刪除 Member 的 Redis
                        JwtClaims jwtClaims = this.mapper.Map<JwtClaims>(memberDao);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            Content = this.jwtService.GenerateToken(jwtClaims)
                        };

                    case (int)UpdateUserInfoResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
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
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 搜尋會員
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="searchType">searchType</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Search(string searchKey, int searchType, string searchMemberID)
        {
            try
            {
                #region 驗證資料

                if (string.IsNullOrEmpty(searchKey))
                {
                    //// 沒有搜尋關鍵字，直接回傳空資料
                    this.logger.LogWarn("搜尋會員失敗，無搜尋關鍵字", $"SearchKey: {searchKey} SearchType: {searchType} SearchMemberID: {searchMemberID}", null);
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = StatusCodes.Status400BadRequest,
                        ResultMessage = ResponseErrorMessageType.SearchKeyEmpty.ToString()
                    };
                }

                #endregion 驗證資料

                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{searchMemberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.Search}-{searchKey}-{searchType}";
                IEnumerable<MemberSimpleInfoView> memberSimpleInfoViews = await this.redisRepository.GetCache<IEnumerable<MemberSimpleInfoView>>(cacheKey).ConfigureAwait(false);
                if (memberSimpleInfoViews == null)
                {
                    bool isFuzzy = searchType.Equals((int)SearchType.Fuzzy);
                    string[] ignoreMemberIds = new string[] { searchMemberID };
                    IEnumerable<MemberDao> memberDaos = await this.memberRepository.Search(searchKey, isFuzzy, ignoreMemberIds).ConfigureAwait(false);
                    memberSimpleInfoViews = await this.TransformMemberSimpleInfoView(memberDaos).ConfigureAwait(false);
                    this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(memberSimpleInfoViews), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = memberSimpleInfoViews
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋會員失敗發生錯誤", $"SearchKey: {searchKey} SearchType: {searchType} SearchMemberID: {searchMemberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得會員名片資訊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetCardInfo(MemberCardInfoContent content, string searchMemberID = null)
        {
            try
            {
                //// TODO 待確認有沒有要限制其他會員可不可以搜尋到資料

                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{content.MemberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.CardInfo}";
                MemberCardInfoView memberCardInfoView = await this.redisRepository.GetCache<MemberCardInfoView>(cacheKey).ConfigureAwait(false);
                if (memberCardInfoView == null)
                {
                    MemberDao memberDao = await this.memberRepository.Get(content.MemberID, MemberSearchType.MemberID).ConfigureAwait(false);
                    if (memberDao == null)
                    {
                        this.logger.LogWarn("取得會員名片資訊失敗，無會員資料", $"Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", null);
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.GetFail.ToString()
                        };
                    }

                    memberCardInfoView = this.mapper.Map<MemberCardInfoView>(memberDao);
                    this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(memberCardInfoView), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = memberCardInfoView
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員名片資訊發生錯誤", $"Content: {JsonConvert.SerializeObject(content)} SearchMemberID: {searchMemberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
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
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.HomeInfo}";
                MemberHomeInfoView memberHomeInfoView = await this.redisRepository.GetCache<MemberHomeInfoView>(cacheKey).ConfigureAwait(false);
                if (memberHomeInfoView == null)
                {
                    MemberDao memberDao = await this.memberRepository.Get(memberID, MemberSearchType.MemberID).ConfigureAwait(false);
                    RideDistanceDao rideDistanceDao = await this.rideRepository.GetTotalDistance(memberID).ConfigureAwait(false);
                    memberHomeInfoView = this.mapper.Map<MemberHomeInfoView>(memberDao);
                    if (rideDistanceDao != null)
                    {
                        memberHomeInfoView.TotalDistance = rideDistanceDao.TotalDistance;
                    }

                    this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(memberHomeInfoView), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.Redis.ExpirationDate));
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = memberHomeInfoView
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得首頁資訊發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 會員手機綁定
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="email">email</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> MobileBind(MemberMobileBindContent content, string memberID, string email)
        {
            try
            {
                #region 驗證資料

                ResponseResult validateVerifyCodeResult = await this.verifyCodeService.Validate(email, content.VerifierCode, false).ConfigureAwait(false);
                if (!validateVerifyCodeResult.Result)
                {
                    this.logger.LogWarn("會員手機綁定失敗，驗證碼錯誤", $"ResultCode: {validateVerifyCodeResult.ResultCode} ResultMessage: {validateVerifyCodeResult.ResultMessage} MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)}", null);
                    return validateVerifyCodeResult;
                }

                #endregion 驗證資料

                #region 檢查手機是否已被綁定

                MemberDao memberDao = await this.memberRepository.Get(content.Mobile, MemberSearchType.Mobile).ConfigureAwait(false);
                if (memberDao != null)
                {
                    this.logger.LogWarn("會員手機綁定失敗，該手機已被綁定", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)} BindMemberID:{memberDao.MemberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.MobileRepeat.ToString()
                    };
                }

                memberDao = await this.memberRepository.Get(memberID, MemberSearchType.MemberID).ConfigureAwait(false);
                if (memberDao == null)
                {
                    this.logger.LogWarn("會員手機綁定失敗，無會員資料", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                    };
                }

                if (!string.IsNullOrEmpty(memberDao.Mobile))
                {
                    this.logger.LogWarn("會員手機綁定失敗，會員已綁定手機", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)} BindMobile:{memberDao.Mobile}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.MobileRepeat.ToString()
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
                this.logger.LogInfo("會員手機綁定結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdateUserInfoResultType.Success:

                        #region 刪除 Redis 驗證碼

                        this.verifyCodeService.Delete(content.VerifierCode);
                        //// TODO 刪除 Member 的 Redis

                        #endregion 刪除 Redis 驗證碼

                        JwtClaims jwtClaims = this.mapper.Map<JwtClaims>(memberDao);
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            Content = this.jwtService.GenerateToken(jwtClaims)
                        };

                    case (int)UpdateUserInfoResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【更新使用者資訊】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員手機綁定發生錯誤", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
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

                ResponseResult validateVerifyCodeResult = await this.verifyCodeService.Validate(content.Email, content.VerifierCode, false).ConfigureAwait(false);
                if (!validateVerifyCodeResult.Result)
                {
                    this.logger.LogWarn("重置會員密碼失敗，驗證碼錯誤", $"ResultCode: {validateVerifyCodeResult.ResultCode} ResultMessage: {validateVerifyCodeResult.ResultMessage} Content: {JsonConvert.SerializeObject(content)}", null);
                    return validateVerifyCodeResult;
                }

                #endregion 驗證資料

                #region 取得會員資料

                MemberDao memberDao = await this.memberRepository.Get(content.Email, MemberSearchType.Email).ConfigureAwait(false);
                if (memberDao == null)
                {
                    this.logger.LogWarn("重置會員密碼失敗，無會員資料", $"Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                    };
                }

                #endregion 取得會員資料

                #region 更新密碼

                MemberUpdatePasswordContent memberUpdatePasswordContent = this.mapper.Map<MemberUpdatePasswordContent>(content);
                ResponseResult responseResult = await this.UpdatePassword(memberUpdatePasswordContent, memberDao.MemberID, true).ConfigureAwait(false);

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
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
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

                MemberDao memberDao = await this.memberRepository.Get(content.Email, MemberSearchType.Email).ConfigureAwait(false);
                if (memberDao == null)
                {
                    this.logger.LogWarn("發送會員忘記密碼驗證碼失敗，無會員資料", $"Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.EmailNotExist.ToString()
                    };
                }

                #endregion 驗證資料

                #region 發送驗證碼

                bool isGenerate = await this.verifyCodeService.IsGenerate(content.Email).ConfigureAwait(false);
                if (isGenerate)
                {
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = StatusCodes.Status200OK,
                        ResultMessage = ResponseSuccessMessageType.SendVerifierCode.ToString()
                    };
                }

                string verifierCode = this.verifyCodeService.Generate(content.Email);
                EmailContext emailContext = EmailContext.GetVerifierCodetEmailContextForForgetPassword(content.Email, verifierCode);
                string postData = JsonConvert.SerializeObject(emailContext);
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.SmtpServer.Domain, AppSettingHelper.Appsetting.SmtpServer.Api, postData).ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("發送會員忘記密碼驗證碼失敗，無法發送郵件", $"Address: {emailContext.Address} Subject: {emailContext.Subject} Body: {emailContext.Body}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status502BadGateway,
                        ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                    };
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    ResultMessage = ResponseSuccessMessageType.SendVerifierCode.ToString()
                };

                #endregion 發送驗證碼
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送會員忘記密碼驗證碼發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 發送會員手機綁定驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="email">email</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> SendMobileBindVerifierCode(MemberRequestMobileBindContent content, string memberID, string email)
        {
            try
            {
                #region 檢查手機是否已被綁定

                MemberDao memberDao = await this.memberRepository.Get(content.Mobile, MemberSearchType.Mobile).ConfigureAwait(false);
                if (memberDao != null)
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼失敗，該手機已被綁定", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)} BindMemberID:{memberDao.MemberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.MobileRepeat.ToString()
                    };
                }

                memberDao = await this.memberRepository.Get(memberID, MemberSearchType.MemberID).ConfigureAwait(false);
                if (memberDao == null)
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼失敗，無會員資料", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                    };
                }

                if (!string.IsNullOrEmpty(memberDao.Mobile))
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼失敗，會員已綁定手機", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)} BindMobile:{memberDao.Mobile}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.MobileRepeat.ToString()
                    };
                }

                #endregion 檢查手機是否已被綁定

                #region 發送驗證碼

                bool isGenerate = await this.verifyCodeService.IsGenerate(email).ConfigureAwait(false);
                if (isGenerate)
                {
                    return new ResponseResult()
                    {
                        Result = true,
                        ResultCode = StatusCodes.Status200OK,
                        ResultMessage = ResponseSuccessMessageType.SendVerifierCode.ToString()
                    };
                }

                string verifierCode = this.verifyCodeService.Generate(email);
                EmailContext emailContext = EmailContext.GetVerifierCodetEmailContextForMobileBind(memberDao.Email, verifierCode);
                string postData = JsonConvert.SerializeObject(emailContext);
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.SmtpServer.Domain, AppSettingHelper.Appsetting.SmtpServer.Api, postData).ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("發送會員手機綁定驗證碼失敗，無法發送郵件", $"Address: {emailContext.Address} Subject: {emailContext.Subject} Body: {emailContext.Body}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status502BadGateway,
                        ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                    };
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    ResultMessage = ResponseSuccessMessageType.SendVerifierCode.ToString()
                };

                #endregion 發送驗證碼
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送會員手機綁定驗證碼發生錯誤", $"MemberID: {memberID} Email: {email} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        /// <summary>
        /// 取得會員詳細資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> GetDetail(string memberID)
        {
            try
            {
                MemberDao memberDao = await this.memberRepository.Get(memberID, MemberSearchType.MemberID).ConfigureAwait(false);
                if (memberDao == null)
                {
                    this.logger.LogWarn("取得會員明細資訊失敗，無會員資料", $"MemberID: {memberID}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = StatusCodes.Status409Conflict,
                        ResultMessage = ResponseErrorMessageType.GetFail.ToString()
                    };
                }

                MemberDao[] memberDaos = new MemberDao[] { memberDao };
                IEnumerable<MemberDetailInfoView> memberDetailInfoViews = await this.TransformMemberDetailInfoView(memberDaos).ConfigureAwait(false);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = StatusCodes.Status200OK,
                    Content = memberDetailInfoViews.FirstOrDefault()
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員明細資訊發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
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

                    string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberDao.MemberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.LastLogin}";
                    MemberDetailInfoView memberDetailInfoView = this.mapper.Map<MemberDetailInfoView>(memberDao);
                    memberDetailInfoView.OnlineType = await this.redisRepository.IsExist(cacheKey, false).ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
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

                    string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.Member}-{memberDao.MemberID}-{AppSettingHelper.Appsetting.Redis.SubFlag.LastLogin}";
                    MemberSimpleInfoView memberSimpleInfoView = this.mapper.Map<MemberSimpleInfoView>(memberDao);
                    memberSimpleInfoView.OnlineType = await this.redisRepository.IsExist(cacheKey, false).ConfigureAwait(false) ? (int)OnlineStatusType.Online : (int)OnlineStatusType.Offline;
                    memberSimpleInfoViews.Add(memberSimpleInfoView);
                }
            }

            return memberSimpleInfoViews;
        }

        /// <summary>
        /// 會員更新密碼
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="isIgnoreOldPassword">isIgnoreOldPassword</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UpdatePassword(MemberUpdatePasswordContent content, string memberID, bool isIgnoreOldPassword)
        {
            try
            {
                #region 發送【更新密碼】指令至後端

                MemberUpdatePasswordRequest request = new MemberUpdatePasswordRequest()
                {
                    MemberID = memberID,
                    Password = content.Password,
                    NewPassword = content.NewPassword,
                    Action = isIgnoreOldPassword ? (int)UpdatePasswordActionType.Forget : (int)UpdatePasswordActionType.Update
                };
                CommandData<MemberEditInfoResponse> response = await this.serverService.DoAction<MemberEditInfoResponse>((int)UserCommandIDType.UpdatePassword, CommandType.User, request).ConfigureAwait(false);
                this.logger.LogInfo("會員更新密碼結果", $"Response: {JsonConvert.SerializeObject(response)} Request: {JsonConvert.SerializeObject(request)}", null);
                switch (response.Data.Result)
                {
                    case (int)UpdatePasswordResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = StatusCodes.Status200OK,
                            ResultMessage = ResponseSuccessMessageType.UpdatePassword.ToString()
                        };

                    case (int)UpdatePasswordResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.UpdateFail.ToString()
                        };

                    case (int)UpdatePasswordResultType.OldPasswordError:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status409Conflict,
                            ResultMessage = ResponseErrorMessageType.OldPasswordError.ToString()
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = StatusCodes.Status502BadGateway,
                            ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                        };
                }

                #endregion 發送【更新密碼】指令至後端
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員更新密碼發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} IsIgnoreOldPassword: {isIgnoreOldPassword}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = StatusCodes.Status500InternalServerError,
                    ResultMessage = ResponseErrorMessageType.SystemError.ToString()
                };
            }
        }

        #endregion 會員資料
    }
}