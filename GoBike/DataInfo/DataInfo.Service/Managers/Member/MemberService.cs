using System;
using System.Threading.Tasks;
using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Resource;
using DataInfo.Repository.Interface;
using DataInfo.Repository.Interface.Sql;
using DataInfo.Repository.Models.Data.Member;
using DataInfo.Service.Interface.Member;
using DataInfo.Service.Models.Member.view;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DataInfo.Service.Managers.Member
{
    /// <summary>
    /// 會員服務
    /// </summary>
    public class MemberService : MembeBaseService, IMemberService
    {
        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public MemberService(IMapper mapper, ISQLMemberRepository memberRepository, IRedisRepository redisRepository) : base(mapper, memberRepository, redisRepository)
        {
        }

        #region 註冊 \ 登入 \ 登出 \ 保持在線

        /// <summary>
        /// 會員保持在線
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="memberID">memberID</param>
        public void KeepOnline(ISession session, string memberID)
        {
            try
            {
                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Member}-{CommonFlagHelper.CommonFlag.RedisFlag.LastLogin}-{memberID}";
                this.redisRepository.UpdateCacheExpire(cacheKey, TimeSpan.FromMinutes(AppSettingHelper.Appsetting.SeesionDeadline));
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員保持在線發生錯誤", $"SessionID: {session.Id} MemberID: {memberID}", ex);
            }
        }

        /// <summary>
        /// 會員登入
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Login(ISession session, string email, string password)
        {
            try
            {
                #region 驗證登入資料

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    this.logger.LogWarn("會員登入結果", $"Result: 驗證失敗 SessionID: {session.Id} Email: {email} Password: {password}", null);
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = "信箱或密碼無效."
                    };
                }

                #endregion 驗證登入資料

                #region 取得資料並驗證密碼

                MemberData memberData = await this.GetMemberData(email).ConfigureAwait(false);
                if (memberData == null)
                {
                    this.logger.LogWarn("會員登入結果", $"Result: 無會員資料 SessionID: {session.Id} Email: {email} Password: {password}", null);
                    return new ResponseResultDto() { Ok = false, Data = "無法根據信箱查詢到相關會員." };
                }

                string decryptAESPassword = Utility.DecryptAES(memberData.Password);
                if (!decryptAESPassword.Equals(password))
                {
                    this.logger.LogWarn("會員登入結果", $"Result: 密碼驗證失敗 SessionID: {session.Id} Email: {email} Password: {password}", null);
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = "密碼驗證失敗."
                    };
                }

                #endregion 取得資料並驗證密碼

                #region 更新最新登入時間

                this.UpdateLastLoginDate(session, memberData);

                #endregion 更新最新登入時間

                return new ResponseResultDto()
                {
                    Ok = true,
                    Data = new MemberLoginInfoViewDto { MemberID = memberData.MemberID, ServerIP = AppSettingHelper.Appsetting.ServerConfig.Ip, ServerPort = AppSettingHelper.Appsetting.ServerConfig.Port }
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員登入發生錯誤", $"SessionID: {session.Id} Email: {email} Password: {password}", ex);
                return new ResponseResultDto()
                {
                    Ok = false,
                    Data = "會員登入發生錯誤."
                };
            }
        }

        /// <summary>
        /// 會員登出
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="memberID">memberID</param>
        public void Logout(ISession session, string memberID)
        {
            try
            {
                session.Remove(CommonFlagHelper.CommonFlag.SessionFlag.MemberID);
                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Member}-{CommonFlagHelper.CommonFlag.RedisFlag.LastLogin}-{memberID}";
                this.redisRepository.DeleteCache(cacheKey);
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員登出發生錯誤", $"SessionID: {session.Id} MemberID: {memberID}", ex);
            }
        }

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <param name="isVerifyPassword">isVerifyPassword</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Register(string email, string password, bool isVerifyPassword, string fbToken, string googleToken)
        {
            try
            {
                string verifyMemberRegisterResult = await this.VerifyMemberRegister(email, password, isVerifyPassword).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(verifyMemberRegisterResult))
                {
                    this.logger.LogWarn("會員註冊結果", $"Result: 驗證失敗({verifyMemberRegisterResult}) Email: {email} Password: {password} IsVerifyPassword: {isVerifyPassword} FbToken: {fbToken} GoogleToken: {googleToken}", null);
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = verifyMemberRegisterResult
                    };
                }

                MemberData memberData = this.CreateMemberData(email, password, fbToken, googleToken);
                bool isSuccess = await this.memberRepository.CreateMemberData(memberData).ConfigureAwait(false);
                this.logger.LogInfo("會員註冊結果", $"Result: {isSuccess} MemberData: {JsonConvert.SerializeObject(memberData)}", null);
                if (isSuccess)
                {
                    return new ResponseResultDto()
                    {
                        Ok = true,
                        Data = "會員註冊成功."
                    };
                }

                return new ResponseResultDto()
                {
                    Ok = false,
                    Data = "會員註冊失敗."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("會員註冊發生錯誤", $"Email: {email} Password: {password} IsVerifyPassword: {isVerifyPassword} FbToken: {fbToken} GoogleToken: {googleToken}", ex);
                return new ResponseResultDto()
                {
                    Ok = false,
                    Data = "會員註冊發生錯誤."
                };
            }
        }

        #region TODO

        /// <summary>
        /// 會員登入 (FB)
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="token">token</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> LoginFB(string email, string token)
        {
            //try
            //{
            //    string postData = JsonConvert.SerializeObject(new MemberDto() { Email = email, FBToken = token });
            //    HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.ServiceDomain.Service, "api/Member/Login/FB", postData);
            //    if (httpResponseMessage.IsSuccessStatusCode)
            //    {
            //        string memberID = await httpResponseMessage.Content.ReadAsAsync<string>();
            //        return new ResponseResultDto()
            //        {
            //            Ok = true,
            //            Data = new string[] { memberID, this.CreateLoginToken(email, string.Empty, token, string.Empty) }
            //        };
            //    }

            //    return new ResponseResultDto()
            //    {
            //        Ok = false,
            //        Data = await httpResponseMessage.Content.ReadAsAsync<string>()
            //    };
            //}
            //catch (Exception ex)
            //{
            //    this.logger.LogError($"Login FB Error >>> FBToken:{token}\n{ex}");
            //    return new ResponseResultDto()
            //    {
            //        Ok = false,
            //        Data = "會員登入發生錯誤."
            //    };
            //}

            return new ResponseResultDto()
            {
                Ok = false,
                Data = "TODO."
            };
        }

        /// <summary>
        /// 會員登入 (Google)
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="token">token</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> LoginGoogle(string email, string token)
        {
            //try
            //{
            //    string postData = JsonConvert.SerializeObject(new MemberDto() { Email = email, GoogleToken = token });
            //    HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.ServiceDomain.Service, "api/Member/Login/Google", postData);
            //    if (httpResponseMessage.IsSuccessStatusCode)
            //    {
            //        string memberID = await httpResponseMessage.Content.ReadAsAsync<string>();
            //        return new ResponseResultDto()
            //        {
            //            Ok = true,
            //            Data = new string[] { memberID, this.CreateLoginToken(email, string.Empty, string.Empty, token) }
            //        };
            //    }

            //    return new ResponseResultDto()
            //    {
            //        Ok = false,
            //        Data = await httpResponseMessage.Content.ReadAsAsync<string>()
            //    };
            //}
            //catch (Exception ex)
            //{
            //    this.logger.LogError($"Login Google Error >>> GoogleToken:{token}\n{ex}");
            //    return new ResponseResultDto()
            //    {
            //        Ok = false,
            //        Data = "會員登入發生錯誤."
            //    };
            //}

            return new ResponseResultDto()
            {
                Ok = false,
                Data = "TODO."
            };
        }

        #endregion TODO

        #endregion 註冊 \ 登入 \ 登出 \ 保持在線

        #region 會員資料

        /// <summary>
        /// 搜尋會員
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Search(dynamic searchKey, string searchMemberID)
        {
            try
            {
                MemberData memberData = await this.GetMemberData(searchKey).ConfigureAwait(false);
                if (memberData == null)
                {
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = "無會員資料."
                    };
                }

                if (!string.IsNullOrEmpty(searchMemberID))
                {
                    if (memberData.MemberID.Equals(searchMemberID))
                    {
                        return new ResponseResultDto()
                        {
                            Ok = false,
                            Data = "無法搜尋會員本人資料."
                        };
                    }
                }

                return new ResponseResultDto()
                {
                    Ok = true,
                    Data = memberData
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("搜尋會員尋發生錯誤", $"SearchKey: {searchKey}", ex);
                return new ResponseResultDto()
                {
                    Ok = false,
                    Data = "搜尋會員發生錯誤."
                };
            }
        }

        #endregion 會員資料
    }
}