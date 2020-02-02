using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Resource;
using DataInfo.Core.Resource.Enum;
using DataInfo.Repository.Interface;
using DataInfo.Repository.Models.Data.Member;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DataInfo.Service.Managers.Member
{
    /// <summary>
    /// 會員基礎服務
    /// </summary>
    public class MembeBaseService
    {
        /// <summary>
        /// logger
        /// </summary>
        protected readonly ILogger<MemberService> logger;

        /// <summary>
        /// mapper
        /// </summary>
        protected readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        protected readonly IMemberRepository memberRepository;

        /// <summary>
        /// redisRepository
        /// </summary>
        protected readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="mapper">mapper</param>
        /// <param name="memberRepository">memberRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public MembeBaseService(ILogger<MemberService> logger, IMapper mapper, IMemberRepository memberRepository, IRedisRepository redisRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.memberRepository = memberRepository;
            this.redisRepository = redisRepository;
        }

        #region 建立功能

        /// <summary>
        /// 建立登入 Token
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>string</returns>
        protected string CreateLoginToken(string email, string password, string fbToken, string googleToken)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                return $"{Utility.EncryptAES(email)}{CommonFlagHelper.CommonFlag.SeparateFlag}{Utility.EncryptAES(password)}";
            }

            if (!string.IsNullOrEmpty(fbToken))
            {
                return $"{Utility.EncryptAES(CommonFlagHelper.CommonFlag.PlatformFlag.FB)}{CommonFlagHelper.CommonFlag.SeparateFlag}{Utility.EncryptAES(email)}{CommonFlagHelper.CommonFlag.SeparateFlag}{Utility.EncryptAES(fbToken)}";
            }

            if (!string.IsNullOrEmpty(googleToken))
            {
                return $"{Utility.EncryptAES(CommonFlagHelper.CommonFlag.PlatformFlag.Google)}{CommonFlagHelper.CommonFlag.SeparateFlag}{Utility.EncryptAES(email)}{CommonFlagHelper.CommonFlag.SeparateFlag}{Utility.EncryptAES(googleToken)}";
            }

            return string.Empty;
        }

        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>MemberData</returns>
        protected MemberData CreateMemberData(string email, string password, string fbToken, string googleToken)
        {
            Random random = new Random();
            return new MemberData()
            {
                MemberID = Convert.ToInt64(random.Next(100001, 999999)), //// TODO 待改由 DB 設定
                RegisterDate = DateTime.Now,
                RegisterSource = string.IsNullOrEmpty(fbToken) ? string.IsNullOrEmpty(googleToken) ? (int)RegisterSourceType.Normal : (int)RegisterSourceType.Google : (int)RegisterSourceType.FB,
                AccountName = email,
                Password = string.IsNullOrEmpty(password) ? string.Empty : Utility.EncryptAES(password),
                FBToken = fbToken,
                GoogleToken = googleToken,
            };
        }

        #endregion 建立功能

        #region 驗證功能

        /// <summary>
        /// Email 格式驗證
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>bool</returns>
        protected bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        /// 驗證密碼格式
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>bool</returns>
        protected bool IsValidPassword(string password)
        {
            //// 待確認驗證方式
            //int passwordCount = password.Length;
            //if (passwordCount < 8 || passwordCount > 14)
            //{
            //    return false;
            //}

            //int preCharCode = -1;
            //for (int i = 0; i < passwordCount; i++)
            //{
            //    string word = password[i].ToString();
            //    int charCode = (int)password[i];
            //    if (!Regex.IsMatch(word, @"[0-9a-zＡ-Ｚ０-９]"))
            //    {
            //        return false;
            //    }

            // if (Math.Abs(charCode - preCharCode) == 1) { return false; }

            //    preCharCode = charCode;
            //}

            return true;
        }

        /// <summary>
        /// 驗證會員註冊資料
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <param name="isVerifyPassword">isVerifyPassword</param>
        /// <returns>string</returns>
        protected async Task<string> VerifyMemberRegister(string email, string password, bool isVerifyPassword)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "信箱無效.";
            }

            if (!this.IsValidEmail(email))
            {
                return "信箱格式錯誤.";
            }

            if (isVerifyPassword)
            {
                if (string.IsNullOrEmpty(password))
                {
                    return "密碼無效.";
                }

                if (!this.IsValidPassword(password))
                {
                    return "密碼格式錯誤.";
                }
            }

            MemberData existMemberData = await this.GetMemberData(email);
            if (existMemberData != null)
            {
                return "此信箱已經被註冊.";
            }

            return string.Empty;
        }

        #endregion 驗證功能

        #region 資料庫功能

        /// <summary>
        /// 新增會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        protected async Task<bool> AddMemberData(MemberData memberData)
        {
            try
            {
                bool isCreateSuccess = await this.memberRepository.CreateMemberData(memberData);
                if (!isCreateSuccess)
                {
                    return false;
                }

                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Member}";
                TaskAwaiter<bool> isSetCacheSuccess = this.redisRepository.SetCache(cacheKey, memberData.AccountName, JsonConvert.SerializeObject(memberData), TimeSpan.FromHours(2)).GetAwaiter();
                isSetCacheSuccess.OnCompleted(() =>
                {
                    if (!isSetCacheSuccess.GetResult())
                    {
                        this.logger.LogWarning(this, "寫入【新增會員資料】快取資料失敗", $"CacheKey:{cacheKey} Email:{memberData.AccountName}");
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "新增會員資料發生錯誤", string.Empty, ex);
                return false;
            }
        }

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <returns>MemberData</returns>
        protected async Task<MemberData> GetMemberData(string email)
        {
            try
            {
                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Member}";
                string redisJsonData = await this.redisRepository.GetCache(cacheKey, email);
                MemberData memberData = null;
                if (string.IsNullOrEmpty(redisJsonData))
                {
                    memberData = await this.memberRepository.GetMemberData(email);
                    if (memberData != null)
                    {
                        TaskAwaiter<bool> isSuccess = this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(memberData), TimeSpan.FromHours(2)).GetAwaiter();
                        isSuccess.OnCompleted(() =>
                        {
                            if (!isSuccess.GetResult())
                            {
                                this.logger.LogWarning(this, "寫入【取得會員資料】快取資料失敗", $"CacheKey:{cacheKey} Email:{email}");
                            }
                        });
                    }
                }
                else
                {
                    memberData = JsonConvert.DeserializeObject<MemberData>(redisJsonData);
                }

                return memberData;
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "取得會員資料發生錯誤", string.Empty, ex);
                return null;
            }
        }

        /// <summary>
        /// 紀錄會員 Session ID
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="memberID">memberID</param>
        /// <returns></returns>
        protected void RecordSessionID(ISession session, long memberID)
        {
            try
            {
                session.SetObject(CommonFlagHelper.CommonFlag.SessionFlag.MemberID, memberID);
                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Session}-{session.Id}-{memberID}";
                TaskAwaiter<bool> isSetCacheSuccess = this.redisRepository.SetCache(cacheKey, memberID.ToString(), TimeSpan.FromMinutes(AppSettingHelper.Appsetting.SeesionDeadline)).GetAwaiter();
                isSetCacheSuccess.OnCompleted(() =>
                {
                    if (!isSetCacheSuccess.GetResult())
                    {
                        this.logger.LogWarning(this, "寫入【紀錄會員 Session ID】快取資料失敗", $"CacheKey:{cacheKey} Session:{session?.Id} MemberID:{memberID}");
                    }
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "紀錄會員 Session ID發生錯誤", $"Session:{session?.Id} MemberID:{memberID}", ex);
            }
        }

        #endregion 資料庫功能
    }
}