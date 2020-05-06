using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using DataInfo.Core.Models.Dao.Member;
using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataInfo.Core.Models.Dao.Ride;
using DataInfo.Core.Models.Dao.Ride.Table;
using DataInfo.Core.Applibs;

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
        /// 取得總里程
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideDistanceDao</returns>
        public async Task<RideDistanceDao> GetTotalDistance(string memberID)
        {
            try
            {
                RideData rideData = await this.Db.Queryable<RideData>().Where(data => data.MemberID.Equals(memberID)).SingleAsync().ConfigureAwait(false);
                if (rideData == null)
                {
                    this.logger.LogWarn("取得總里程失敗", $"Result: 無騎乘資料 MemberID: {memberID}", null);
                    return null;
                }

                return new RideDistanceDao()
                {
                    MemberID = rideData.MemberID,
                    TotalDistance = rideData.TotalDistance
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得總里程發生錯誤", $"MemberID: {memberID}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得週里程
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="startDate">startDate</param>
        /// <param name="endDate">endDate</param>
        /// <returns>RideDistanceDao</returns>
        public async Task<RideDistanceDao> GetWeekDistance(string memberID, DateTime startDate, DateTime endDate)
        {
            try
            {
                string weekFirstDay = Utility.GetWeekFirstDay(startDate);
                string weekLastDay = Utility.GetWeekLastDay(endDate);
                WeekRideData weekRideData = await this.Db.Queryable<WeekRideData>().Where(data => data.MemberID.Equals(memberID))
                                                                           .Where(data => data.WeekFirstDay.Equals(weekFirstDay) && data.WeekLastDay.Equals(weekLastDay)).SingleAsync().ConfigureAwait(false);
                if (weekRideData == null)
                {
                    this.logger.LogWarn("取得週里程失敗", $"Result: 無騎乘資料 MemberID: {memberID} StartDate: {startDate} EndDate: {endDate}", null);
                    return null;
                }

                return new RideDistanceDao()
                {
                    MemberID = weekRideData.MemberID,
                    WeekDistance = weekRideData.WeekDistance
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得週里程發生錯誤", $"MemberID: {memberID} StartDate: {startDate} EndDate: {endDate}", ex);
                return null;
            }
        }

        ///// <summary>
        ///// 取得騎乘資料
        ///// </summary>
        ///// <param name="memberID">memberID</param>
        ///// <returns>RideModel</returns>
        //public async Task<RideModel> Get(string rideID)
        //{
        //    try
        //    {
        //        return await this.Db.Queryable<RideModel>().Where(data => data.RideID.Equals(rideID)).SingleAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("取得騎乘資料發生錯誤", $"RideID: {rideID}", ex);
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 取得會員的騎乘資料列表
        ///// </summary>
        ///// <param name="memberID">memberID</param>
        ///// <returns>RideModel list</returns>
        //public async Task<List<RideModel>> GetListOfMember(string memberID)
        //{
        //    try
        //    {
        //        return await this.Db.Queryable<RideModel>().Where(data => data.MemberID.Equals(memberID)).ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("取得會員的騎乘資料列表發生錯誤", $"MemberID: {memberID}", ex);
        //        return new List<RideModel>();
        //    }
        //}

        ///// <summary>
        ///// 更新騎乘資料
        ///// </summary>
        ///// <param name="rideModel">rideModel</param>
        ///// <returns>bool</returns>
        //public async Task<bool> Update(RideModel rideModel)
        //{
        //    try
        //    {
        //        bool isSuccess = await this.Db.Updateable(rideModel)
        //                                      .With(SqlWith.HoldLock)
        //                                      .With(SqlWith.UpdLock)
        //                                      .ExecuteCommandAsync() > 0;
        //        this.logger.LogInfo("更新騎乘資料結果", $"Result: {isSuccess} RideModel: {JsonConvert.SerializeObject(rideModel)}", null);
        //        return isSuccess;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("更新騎乘資料發生錯誤", $"RideModel: {JsonConvert.SerializeObject(rideModel)}", ex);
        //        return false;
        //    }
        //}
    }
}