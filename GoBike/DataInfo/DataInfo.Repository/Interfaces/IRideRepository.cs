using DataInfo.Core.Models.Dao.Ride;
using System;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces
{
    /// <summary>
    /// 騎乘資料庫
    /// </summary>
    public interface IRideRepository
    {
        /// <summary>
        /// 取得總里程
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideDistanceDao</returns>
        Task<RideDistanceDao> GetTotalDistance(string memberID);

        /// <summary>
        /// 取得週里程
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="startDate">startDate</param>
        /// <param name="endDate">endDate</param>
        /// <returns>RideDistanceDao</returns>
        Task<RideDistanceDao> GetWeekDistance(string memberID, DateTime startDate, DateTime endDate);
    }
}