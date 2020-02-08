using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Resource;
using DataInfo.Core.Resource.Enum;
using DataInfo.Repository.Interface;
using DataInfo.Repository.Interface.Sql;
using DataInfo.Repository.Models.Data.Member;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;

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
        protected readonly ILogger logger = LogManager.GetLogger("MemberService");

        /// <summary>
        /// mapper
        /// </summary>
        protected readonly IMapper mapper;

        /// <summary>
        /// memberRepository
        /// </summary>
        protected readonly ISQLMemberRepository memberRepository;

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
        public MembeBaseService(IMapper mapper, ISQLMemberRepository memberRepository, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.memberRepository = memberRepository;
            this.redisRepository = redisRepository;
        }

        #region 建立功能

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
            byte[] dateTimeBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
            Array.Resize(ref dateTimeBytes, 16);
            string memberID = new Guid(dateTimeBytes).ToString().Substring(0, 8);
            return new MemberData()
            {
                MemberID = memberID,
                RegisterDate = DateTime.UtcNow,
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

            MemberData existMemberData = await this.GetMemberData(email).ConfigureAwait(false);
            if (existMemberData != null)
            {
                return "此信箱已經被註冊.";
            }

            return string.Empty;
        }

        #endregion 驗證功能

        #region 資料庫功能

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <returns>MemberData</returns>
        protected async Task<MemberData> GetMemberData(string searchKey)
        {
            try
            {
                this.logger.LogInfo("取得會員資料", $"SearchKey: {searchKey}", null);
                if (searchKey.Contains("@"))
                {
                    return await this.memberRepository.GetMemberDataByEmail(searchKey).ConfigureAwait(false);
                }
                else if (searchKey.Length == 8) //// 目前只能先寫死，待思考有沒有其他更好的方式
                {
                    return await this.memberRepository.GetMemberDataByMemberID(searchKey).ConfigureAwait(false);
                }

                return null;
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤", $"SearchKey: {searchKey}", ex);
                return null;
            }
        }

        /// <summary>
        /// 更新最新登入時間
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="memberData">memberData</param>
        protected void UpdateLastLoginDate(ISession session, MemberData memberData)
        {
            try
            {
                this.logger.LogInfo("更新會員最新登入時間", $"MemberID: {memberData.MemberID}", null);
                session.SetObject(CommonFlagHelper.CommonFlag.SessionFlag.MemberID, memberData.MemberID);
                memberData.LoginDate = DateTime.UtcNow;
                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Member}-{CommonFlagHelper.CommonFlag.RedisFlag.LastLogin}-{memberData.MemberID}";
                this.redisRepository.SetCache(cacheKey, $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}", TimeSpan.FromMinutes(AppSettingHelper.Appsetting.SeesionDeadline));
                this.memberRepository.UpdateMemberData(memberData);
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新會員最新登入時間發生錯誤", $"MemberData: {JsonConvert.SerializeObject(memberData)}", ex);
            }
        }

        #endregion 資料庫功能
    }
}