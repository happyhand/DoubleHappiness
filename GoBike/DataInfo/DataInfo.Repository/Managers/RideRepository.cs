using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using DataInfo.Repository.Models.Member;
using Newtonsoft.Json;
using NLog;
using SqlSugar;

namespace DataInfo.Repository.Managers
{
    /// <summary>
    /// 騎乘資料庫
    /// </summary>
    public class RideRepository : MainBase, IRideRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RideRepository");

        /// <summary>
        /// 建立騎乘資料
        /// </summary>
        /// <param name="rideModel">rideModel</param>
        /// <returns>bool</returns>
        public async Task<bool> Create(RideModel rideModel)
        {
            try
            {
                bool isSuccess = await this.Db.Insertable(rideModel)
                                              .With(SqlWith.HoldLock)
                                              .With(SqlWith.UpdLock)
                                              .ExecuteCommandAsync() > 0;
                this.logger.LogInfo("建立騎乘資料結果", $"Result: {isSuccess} RideModel: {JsonConvert.SerializeObject(rideModel)}", null);
                return isSuccess;
            }
            catch (Exception ex)
            {
                this.logger.LogError("建立騎乘資料發生錯誤", $"RideModel: {JsonConvert.SerializeObject(rideModel)}", ex);
                return false;
            }
        }

        /// <summary>
        /// 取得騎乘資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideModel</returns>
        public async Task<RideModel> Get(string rideID)
        {
            try
            {
                return await this.Db.Queryable<RideModel>().Where(data => data.RideID.Equals(rideID)).SingleAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得騎乘資料發生錯誤", $"RideID: {rideID}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得會員的騎乘資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideModel list</returns>
        public async Task<List<RideModel>> GetListOfMember(string memberID)
        {
            try
            {
                return await this.Db.Queryable<RideModel>().Where(data => data.MemberID.Equals(memberID)).ToListAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員的騎乘資料列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<RideModel>();
            }
        }

        /// <summary>
        /// 更新騎乘資料
        /// </summary>
        /// <param name="rideModel">rideModel</param>
        /// <returns>bool</returns>
        public async Task<bool> Update(RideModel rideModel)
        {
            try
            {
                bool isSuccess = await this.Db.Updateable(rideModel)
                                              .With(SqlWith.HoldLock)
                                              .With(SqlWith.UpdLock)
                                              .ExecuteCommandAsync() > 0;
                this.logger.LogInfo("更新騎乘資料結果", $"Result: {isSuccess} RideModel: {JsonConvert.SerializeObject(rideModel)}", null);
                return isSuccess;
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新騎乘資料發生錯誤", $"RideModel: {JsonConvert.SerializeObject(rideModel)}", ex);
                return false;
            }
        }
    }
}