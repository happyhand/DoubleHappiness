using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Ride;
using DataInfo.Core.Models.Dao.Ride.Table;
using DataInfo.Repository.Interfaces.Common;
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
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        public RideRepository(IMapper mapper, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 取得指定騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="rideID">rideID</param>
        /// <returns>RideDao</returns>
        public async Task<RideDao> Get(string memberID, string rideID)
        {
            try
            {
                using (SqlSugarClient db = this.NewDB)
                {
                    RideRecord rideRecord = await db.Queryable<RideRecord>()
                                                         .Where(data => data.MemberID.Equals(memberID))
                                                         .Where(data => data.RideID.Equals(rideID))
                                                         .FirstAsync().ConfigureAwait(false);

                    return this.mapper.Map<RideDao>(rideRecord);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得指定騎乘記錄發生錯誤", $"RideID: {rideID}", ex);
                return null;
            }
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
                using (SqlSugarClient db = this.NewDB)
                {
                    IEnumerable<RideRecord> rideRecords = await db.Queryable<RideRecord>()
                                                         .Where(data => data.MemberID.Equals(memberID))
                                                         .ToListAsync().ConfigureAwait(false);

                    return this.mapper.Map<IEnumerable<RideDao>>(rideRecords);
                }
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
                using (SqlSugarClient db = this.NewDB)
                {
                    string weekFirstDay = Utility.GetWeekFirstDay(nowDate);
                    string weekLastDay = Utility.GetWeekLastDay(nowDate);
                    WeekRideData weekRideData = await db.Queryable<WeekRideData>()
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
                using (SqlSugarClient db = this.NewDB)
                {
                    string weekFirstDay = Utility.GetWeekFirstDay(nowDate);
                    string weekLastDay = Utility.GetWeekLastDay(nowDate);
                    IEnumerable<WeekRideData> weekRideDatas = await db.Queryable<WeekRideData>()
                                                             .Where(data => memberIDs.Contains(data.MemberID))
                                                             .Where(data => data.WeekFirstDay.Equals(weekFirstDay) && data.WeekLastDay.Equals(weekLastDay))
                                                             .ToListAsync().ConfigureAwait(false);

                    return weekRideDatas.Select(data => new RideDistanceDao()
                    {
                        MemberID = data.MemberID,
                        WeekDistance = data.WeekDistance
                    });
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得週里程發生錯誤", $"MemberIDs: {JsonConvert.SerializeObject(memberIDs)} NowDate: {nowDate}", ex);
                return new List<RideDistanceDao>();
            }
        }

        /// <summary>
        /// 取得組隊騎乘路線
        /// </summary>
        /// <param name="rideID">rideID</param>
        /// <param name="index">index</param>
        /// <returns>RideRouteDao</returns>
        public async Task<RideRouteDao> GetRideRoute(string rideID, int index)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.RideRouteCount}_{rideID}";
                RideRouteCountDao rideRouteCountDao = await this.redisRepository.GetCache<RideRouteCountDao>(AppSettingHelper.Appsetting.Redis.RideDB, cacheKey).ConfigureAwait(false);
                if (rideRouteCountDao == null)
                {
                    this.logger.LogWarn("取得組隊騎乘路線失敗，無組隊騎乘路線索引資料", $"RideID: {rideID} Index: {index}", null);
                    return null;
                }

                cacheKey = $"{rideID}{AppSettingHelper.Appsetting.Redis.Flag.RideRouteInfo}_{index}";
                RideRouteInfoDao rideRouteInfoDao = await this.redisRepository.GetCache<RideRouteInfoDao>(AppSettingHelper.Appsetting.Redis.RideDB, cacheKey).ConfigureAwait(false);
                if (rideRouteInfoDao == null)
                {
                    this.logger.LogWarn("取得組隊騎乘路線失敗，無組隊騎乘路線資訊資料", $"RideID: {rideID} Index: {index}", null);
                    return null;
                }


                return new RideRouteDao()
                {
                    MemberID = rideRouteInfoDao.MemberID,
                    IndexCount = rideRouteCountDao.IndexCount,
                    Index = rideRouteInfoDao.Index,
                    Route = JsonConvert.DeserializeObject<IEnumerable<IEnumerable<string>>>(rideRouteInfoDao.Route),
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得組隊騎乘路線發生錯誤", $"RideID: {rideID} Index: {index}", ex);
                return null;
            }
        }
    }
}