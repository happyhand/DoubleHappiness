using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Ride;
using DataInfo.Core.Models.Dao.Ride.Table;
using DataInfo.Repository.Interfaces.Ride;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers.Ride
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
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        public RideRepository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// 取得騎乘記錄列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideDaos</returns>
        public async Task<IEnumerable<RideDao>> GetRecordList(string memberID)
        {
            try
            {
                IEnumerable<RideRecord> rideRecords = await this.Db.Queryable<RideRecord>()
                                                         .Where(data => data.MemberID.Equals(memberID))
                                                         .ToListAsync().ConfigureAwait(false);

                return this.mapper.Map<IEnumerable<RideDao>>(rideRecords);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得騎乘記錄列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<RideDao>();
            }
        }

        /// <summary>
        /// 取得總里程
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideDistanceDao</returns>
        public async Task<RideDistanceDao> GetTotalDistance(string memberID)
        {
            try
            {
                using (SqlSugarClient db = this.NewDB)
                {
                    RideData rideData = await db.Queryable<RideData>().Where(data => data.MemberID.Equals(memberID)).FirstAsync().ConfigureAwait(false);
                    if (rideData == null)
                    {
                        this.logger.LogWarn("取得總里程失敗，無騎乘資料", $"MemberID: {memberID}", null);
                        return null;
                    }

                    return new RideDistanceDao()
                    {
                        MemberID = rideData.MemberID,
                        TotalDistance = rideData.TotalDistance
                    };
                }
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
        /// <returns>RideDistanceDao</returns>
        public async Task<RideDistanceDao> GetWeekDistance(string memberID)
        {
            DateTime nowDate = DateTime.UtcNow;
            try
            {
                string weekFirstDay = Utility.GetWeekFirstDay(nowDate);
                string weekLastDay = Utility.GetWeekLastDay(nowDate);
                WeekRideData weekRideData = await this.Db.Queryable<WeekRideData>()
                                                         .Where(data => data.MemberID.Equals(memberID))
                                                         .Where(data => data.WeekFirstDay.Equals(weekFirstDay) && data.WeekLastDay.Equals(weekLastDay))
                                                         .FirstAsync().ConfigureAwait(false);
                if (weekRideData == null)
                {
                    this.logger.LogWarn("取得週里程失敗，無騎乘資料", $"MemberID: {memberID} NowDate: {nowDate}", null);
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
                this.logger.LogError("取得週里程發生錯誤", $"MemberID: {memberID} NowDate: {nowDate}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得週里程
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>RideDistanceDaos</returns>
        public async Task<IEnumerable<RideDistanceDao>> GetWeekDistance(IEnumerable<string> memberIDs)
        {
            DateTime nowDate = DateTime.UtcNow;
            try
            {
                string weekFirstDay = Utility.GetWeekFirstDay(nowDate);
                string weekLastDay = Utility.GetWeekLastDay(nowDate);
                IEnumerable<WeekRideData> weekRideDatas = await this.Db.Queryable<WeekRideData>()
                                                         .Where(data => memberIDs.Contains(data.MemberID))
                                                         .Where(data => data.WeekFirstDay.Equals(weekFirstDay) && data.WeekLastDay.Equals(weekLastDay))
                                                         .ToListAsync().ConfigureAwait(false);

                return weekRideDatas.Select(data => new RideDistanceDao()
                {
                    MemberID = data.MemberID,
                    WeekDistance = data.WeekDistance
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得週里程發生錯誤", $"MemberIDs: {JsonConvert.SerializeObject(memberIDs)} NowDate: {nowDate}", ex);
                return new List<RideDistanceDao>();
            }
        }
    }
}