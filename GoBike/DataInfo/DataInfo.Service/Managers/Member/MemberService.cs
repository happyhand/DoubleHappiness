using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Resource;
using DataInfo.Repository.Interface;
using DataInfo.Repository.Models.Data.Member;
using DataInfo.Service.Interface.Member;
using DataInfo.Service.Models.Member;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        /// <param name="logger">logger</param>
        /// <param name="mapper">mapper</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public MemberService(ILogger<MemberService> logger, IMapper mapper, IMemberRepository memberRepository, IRedisRepository redisRepository) : base(logger, mapper, memberRepository, redisRepository)
        {
        }

        #region 註冊\登入

        /// <summary>
        /// 刪除會員 Session ID
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="sessionID">sessionID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> DeleteSessionID(string memberID, string sessionID)
        {
            //try
            //{
            //    string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Session}-{sessionID}-{memberID}";
            //    bool result = await this.redisRepository.DeleteCache(cacheKey);
            //    if (result)
            //    {
            //        return new ResponseResultDto()
            //        {
            //            Ok = true,
            //            Data = "刪除會員 Session ID 成功."
            //        };
            //    }

            //    return new ResponseResultDto()
            //    {
            //        Ok = false,
            //        Data = "刪除會員 Session ID 失敗."
            //    };
            //}
            //catch (Exception ex)
            //{
            //    this.logger.LogError($"Delete Session ID Error >>> MemberID:{memberID} SessionID:{sessionID}\n{ex}");
            //    return new ResponseResultDto()
            //    {
            //        Ok = false,
            //        Data = "刪除會員 Session ID 發生錯誤."
            //    };
            //}

            return new ResponseResultDto()
            {
                Ok = false,
                Data = "TODO."
            };
        }

        /// <summary>
        /// 延長會員 Session ID 期限
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="sessionID">sessionID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> ExtendSessionIDExpire(string memberID, string sessionID)
        {
            //try
            //{
            //    string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Session}-{sessionID}-{memberID}";
            //    bool result = await this.redisRepository.UpdateCacheExpire(cacheKey, TimeSpan.FromMinutes(AppSettingHelper.Appsetting.SeesionDeadline));
            //    if (result)
            //    {
            //        return new ResponseResultDto()
            //        {
            //            Ok = true,
            //            Data = "延長 Session ID 成功."
            //        };
            //    }

            //    return new ResponseResultDto()
            //    {
            //        Ok = false,
            //        Data = "延長 Session ID 失敗."
            //    };
            //}
            //catch (Exception ex)
            //{
            //    this.logger.LogError($"Extend Session ID Expire Error >>> MemberID:{memberID} SessionID:{sessionID}\n{ex}");
            //    return new ResponseResultDto()
            //    {
            //        Ok = false,
            //        Data = "延長 Session ID 發生錯誤."
            //    };
            //}

            return new ResponseResultDto()
            {
                Ok = false,
                Data = "TODO."
            };
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
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = "信箱或密碼無效."
                    };
                }

                MemberData memberData = await this.GetMemberData(email);
                if (memberData == null)
                {
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = "無法根據信箱查詢到相關會員."
                    };
                }

                string decryptAESPassword = string.IsNullOrEmpty(memberData.Password) ? string.Empty : Utility.DecryptAES(memberData.Password);
                if (string.IsNullOrEmpty(decryptAESPassword) || !decryptAESPassword.Equals(password))
                {
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = "密碼驗證失敗."
                    };
                }

                string token = this.CreateLoginToken(email, password, string.Empty, string.Empty);
                this.RecordSessionID(session, memberData.MemberID);
                return new ResponseResultDto()
                {
                    Ok = true,
                    Data = new MemberLoginInfoViewDto { MemberID = memberData.MemberID, Token = token, ServerIP = AppSettingHelper.Appsetting.ServerConfig.Ip, ServerPort = AppSettingHelper.Appsetting.ServerConfig.Port }
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Login Error >>> Email:{email} Password:{password}\n{ex}");
                return new ResponseResultDto()
                {
                    Ok = false,
                    Data = "會員登入發生錯誤."
                };
            }
        }

        /// <summary>
        /// 會員登入 (token)
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Login(string token)
        {
            //try
            //{
            //    string[] dataArr = token.Split(CommonFlagHelper.CommonFlag.SeparateFlag);
            //    string data1 = Utility.DecryptAES(dataArr[0]);
            //    string data2 = Utility.DecryptAES(dataArr[1]);
            //    if (data1.Equals(CommonFlagHelper.CommonFlag.PlatformFlag.FB))
            //    {
            //        string fbToken = Utility.DecryptAES(dataArr[2]);
            //        return await this.LoginFB(data2, fbToken);
            //    }

            // if (data1.Equals(CommonFlagHelper.CommonFlag.PlatformFlag.Google)) { string
            // googleToken = Utility.DecryptAES(dataArr[2]); return await this.LoginGoogle(data2,
            // googleToken); }

            //    return await this.Login(data1, data2);
            //}
            //catch (Exception ex)
            //{
            //    this.logger.LogError($"Login Auto Token Error >>> Token:{token}\n{ex}");
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

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Register(string email, string password, bool isVerifyPassword, string fbToken, string googleToken)
        {
            try
            {
                string verifyMemberRegisterResult = await this.VerifyMemberRegister(email, password, isVerifyPassword);
                if (!string.IsNullOrEmpty(verifyMemberRegisterResult))
                {
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = verifyMemberRegisterResult
                    };
                }

                MemberData memberData = this.CreateMemberData(email, password, fbToken, googleToken);
                bool isSuccess = await this.AddMemberData(memberData);
                if (!isSuccess)
                {
                    return new ResponseResultDto()
                    {
                        Ok = false,
                        Data = "會員註冊失敗."
                    };
                }

                return new ResponseResultDto()
                {
                    Ok = true,
                    Data = "會員註冊成功."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "會員註冊發生錯誤", $"Email:{email} Password:{password}", ex);
                return new ResponseResultDto()
                {
                    Ok = false,
                    Data = "會員註冊發生錯誤."
                };
            }
        }

        #endregion 註冊\登入

        #region 會員資料

        /// <summary>
        /// 會員搜尋
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> Search(dynamic searchKey)
        {
            try
            {
                MemberData memberData = null;
                //// 判斷 Search Key
                if (searchKey is string && searchKey.Contains("@"))
                {
                    memberData = await this.GetMemberData(searchKey);
                    if (memberData != null)
                    {
                        return new ResponseResultDto()
                        {
                            Ok = true,
                            Data = memberData
                        };
                    }
                }
                else if (searchKey is long) //// 目前只能先寫死，待思考有沒有其他更好的方式
                {
                }

                return new ResponseResultDto()
                {
                    Ok = false,
                    Data = "會員搜尋參數無效."
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "會員搜尋發生錯誤", $"SearchKey:{searchKey}", ex);
                return new ResponseResultDto()
                {
                    Ok = false,
                    Data = "會員搜尋發生錯誤."
                };
            }
        }

        #endregion 會員資料
    }
}