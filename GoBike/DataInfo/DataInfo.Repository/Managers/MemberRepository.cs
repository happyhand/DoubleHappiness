using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using DataInfo.Repository.Models.Member;
using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers
{
    /// <summary>
    /// 會員資料庫
    /// </summary>
    public class MemberRepository : MainBase, IMemberRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberRepository");

        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="memberModel">memberModel</param>
        /// <returns>bool</returns>
        public async Task<bool> Create(MemberModel memberModel)
        {
            try
            {
                bool isSuccess = await this.Db.Insertable(memberModel)
                                              .With(SqlWith.HoldLock)
                                              .With(SqlWith.UpdLock)
                                              .ExecuteCommandAsync()
                                              .ConfigureAwait(false) > 0;
                this.logger.LogInfo("建立會員資料結果", $"Result: {isSuccess} MemberModel: {JsonConvert.SerializeObject(memberModel)}", null);
                return isSuccess;
            }
            catch (Exception ex)
            {
                this.logger.LogError("建立會員資料發生錯誤", $"MemberModel: {JsonConvert.SerializeObject(memberModel)}", ex);
                return false;
            }
        }

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>MemberModels</returns>
        public async Task<IEnumerable<MemberModel>> Get(string searchKey, bool isFuzzy)
        {
            try
            {
                if (isFuzzy)
                {
                    return await this.Db.Queryable<MemberModel>()
                        .Where(data => data.Email.Contains(searchKey) || data.Nickname.Contains(searchKey) || data.MemberID.Contains(searchKey))
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    MemberModel memberModel = null;
                    if (Utility.ValidateEmail(searchKey))
                    {
                        memberModel = await this.Db.Queryable<MemberModel>().Where(data => data.Email.Equals(searchKey))
                                                           .SingleAsync()
                                                           .ConfigureAwait(false);
                    }
                    else if (searchKey.Contains(AppSettingHelper.Appsetting.MemberIDFlag))
                    {
                        memberModel = await this.Db.Queryable<MemberModel>().Where(data => data.MemberID.Equals(searchKey))
                                                          .SingleAsync()
                                                          .ConfigureAwait(false);
                    }

                    List<MemberModel> list = new List<MemberModel>();
                    if (memberModel != null)
                    {
                        list.Add(memberModel);
                    };

                    return list;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤", $"SearchKey: {searchKey} IsFuzzy: {isFuzzy}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得會員資料 (By Email)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>MemberModel</returns>
        public async Task<MemberModel> GetByEmail(string email)
        {
            try
            {
                return await this.Db.Queryable<MemberModel>().Where(data => data.Email.Equals(email))
                                                             .SingleAsync()
                                                             .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤(Email)", $"Email: {email}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得會員資料 (模糊查詢)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>MemberModels</returns>
        public async Task<IEnumerable<MemberModel>> GetByFuzzy(string searchKey)
        {
            try
            {
                return await this.Db.Queryable<MemberModel>().Where(data => data.Email.Contains(searchKey) || data.Nickname.Contains(searchKey) || data.MemberID.Contains(searchKey))
                                                             .ToListAsync()
                                                             .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員資料發生錯誤(模糊查詢)", $"SearchKey: {searchKey}", ex);
                return null;
            }
        }

        ///// <summary>
        ///// 取得會員資料 (By FB)
        ///// </summary>
        ///// <param name="fbToken">fbToken</param>
        ///// <returns>MemberData</returns>
        //public async Task<Member> GetMemberDataByFB(string fbToken)
        //{
        //    try
        //    {
        //        using (var maindb = new Maindb(this.serviceProvider.GetRequiredService<DbContextOptions<Maindb>>()))
        //        {
        //            return await maindb.Member.FirstOrDefaultAsync(options => options.FBToken.Equals(fbToken)).ConfigureAwait(false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("取得會員資料發生錯誤(FB)", $"FBToken: {fbToken}", ex);
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 取得會員資料 (By Google)
        ///// </summary>
        ///// <param name="googleToken">googleToken</param>
        ///// <returns>MemberData</returns>
        //public async Task<Member> GetMemberDataByGoogle(string googleToken)
        //{
        //    try
        //    {
        //        using (var maindb = new Maindb(this.serviceProvider.GetRequiredService<DbContextOptions<Maindb>>()))
        //        {
        //            return await maindb.Member.FirstOrDefaultAsync(options => options.GoogleToken.Equals(googleToken)).ConfigureAwait(false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("取得會員資料發生錯誤(Google)", $"GoogleToken: {googleToken}", ex);
        //        return null;
        //    }
        //}

        /// <summary>
        /// 取得會員資料 (By MemberID)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberModel</returns>
        public async Task<MemberModel> GetByMemberID(string memberID)
        {
            try
            {
                return await this.Db.Queryable<MemberModel>().Where(data => data.MemberID.Equals(memberID))
                                                             .SingleAsync()
                                                             .ConfigureAwait(false);
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
        /// <param name="memberModel">memberModel</param>
        /// <returns>bool</returns>
        public async Task<bool> Update(MemberModel memberModel)
        {
            try
            {
                bool isSuccess = await this.Db.Updateable(memberModel)
                                              .With(SqlWith.HoldLock)
                                              .With(SqlWith.UpdLock)
                                              .ExecuteCommandAsync()
                                              .ConfigureAwait(false) > 0;
                this.logger.LogInfo("更新會員資料結果", $"Result: {isSuccess} MemberModel: {JsonConvert.SerializeObject(memberModel)}", null);
                return isSuccess;
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新會員資料發生錯誤", $"MemberModel: {JsonConvert.SerializeObject(memberModel)}", ex);
                return false;
            }
        }

        /// <summary>
        /// 更新多筆會員資料
        /// </summary>
        /// <param name="memberModels">memberModels</param>
        /// <returns>bool</returns>
        public async Task<bool> Update(List<MemberModel> memberModels)
        {
            try
            {
                bool isSuccess = await this.Db.Updateable(memberModels)
                                              .With(SqlWith.HoldLock)
                                              .With(SqlWith.UpdLock)
                                              .ExecuteCommandAsync()
                                              .ConfigureAwait(false) > 0;
                this.logger.LogInfo("更新多筆會員資料結果", $"Result: {isSuccess} MemberModels: {JsonConvert.SerializeObject(memberModels)}", null);
                return isSuccess;
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新多筆會員資料發生錯誤", $"MemberModels: {JsonConvert.SerializeObject(memberModels)}", ex);
                return false;
            }
        }
    }
}