using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interface.Sql;
using DataInfo.Repository.Models.Data.Member;
using DataInfo.Repository.Models.Sql.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;

namespace DataInfo.Repository.Managers.Sql
{
    public class SQLMemberRepository : ISQLMemberRepository
    {
        private readonly ILogger logger = LogManager.GetLogger("SQLMemberRepository");

        /// <summary>
        /// SQL DB
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        public SQLMemberRepository(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        public async Task<bool> CreateMemberData(MemberData memberData)
        {
            try
            {
                using (var maindb = new Maindb(this.serviceProvider.GetRequiredService<DbContextOptions<Maindb>>()))
                {
                    await maindb.Member.AddAsync(memberData).ConfigureAwait(false);
                    bool isSuccess = await maindb.SaveChangesAsync().ConfigureAwait(false) > 0;
                    this.logger.LogInfo("建立會員資料", $"Result: {isSuccess} MemberData: {JsonConvert.SerializeObject(memberData)}", null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("建立會員資料發生錯誤", $"MemberData: {JsonConvert.SerializeObject(memberData)}", ex);
                return false;
            }
        }

        /// <summary>
        /// 取得會員資料 (By Email)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberDataByEmail(string email)
        {
            try
            {
                using (var maindb = new Maindb(this.serviceProvider.GetRequiredService<DbContextOptions<Maindb>>()))
                {
                    return await maindb.Member.FirstOrDefaultAsync(options => options.AccountName.Equals(email)).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤(Email)", $"Email: {email}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得會員資料 (By FB)
        /// </summary>
        /// <param name="fbToken">fbToken</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberDataByFB(string fbToken)
        {
            try
            {
                using (var maindb = new Maindb(this.serviceProvider.GetRequiredService<DbContextOptions<Maindb>>()))
                {
                    return await maindb.Member.FirstOrDefaultAsync(options => options.FBToken.Equals(fbToken)).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤(FB)", $"FBToken: {fbToken}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得會員資料 (By Google)
        /// </summary>
        /// <param name="googleToken">googleToken</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberDataByGoogle(string googleToken)
        {
            try
            {
                using (var maindb = new Maindb(this.serviceProvider.GetRequiredService<DbContextOptions<Maindb>>()))
                {
                    return await maindb.Member.FirstOrDefaultAsync(options => options.GoogleToken.Equals(googleToken)).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤(Google)", $"GoogleToken: {googleToken}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得會員資料 (By MemberID)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberDataByMemberID(string memberID)
        {
            try
            {
                using (var maindb = new Maindb(this.serviceProvider.GetRequiredService<DbContextOptions<Maindb>>()))
                {
                    return await maindb.Member.FirstOrDefaultAsync(options => options.MemberID.Equals(memberID)).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤(MemberID)", $"MemberID: {memberID}", ex);
                return null;
            }
        }

        /// <summary>
        /// 更新會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateMemberData(MemberData memberData)
        {
            try
            {
                using (var maindb = new Maindb(this.serviceProvider.GetRequiredService<DbContextOptions<Maindb>>()))
                {
                    maindb.Update(memberData);
                    bool isSuccess = await maindb.SaveChangesAsync().ConfigureAwait(false) > 0;
                    this.logger.LogInfo("更新會員資料", $"Result: {isSuccess} MemberData: {JsonConvert.SerializeObject(memberData)}", null);
                    return isSuccess;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新會員資料發生錯誤", $"MemberData: {JsonConvert.SerializeObject(memberData)}", ex);
                return false;
            }
        }
    }
}