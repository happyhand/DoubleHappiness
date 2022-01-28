using DataInfo.Core.Models.Dao.Ride;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces.Ride
{
    /// <summary>
    /// 騎乘資料庫
    /// </summary>
    public interface IRideRepository
    {
        /// <summary>
        /// 取得指定騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="rideID">rideID</param>
        /// <returns>RideDao</returns>
        Task<RideDao> Get(string memberID, string rideID);

        /// <summary>
        /// 取得騎乘記錄列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideDaos</returns>
        Task<IEnumerable<RideDao>> GetRecordList(string memberID);

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
        /// <returns>RideDistanceDao</returns>
        Task<RideDistanceDao> GetWeekDistance(string memberID);

        /// <summary>
        /// 取得週里程
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>RideDistanceDaos</returns>
        Task<IEnumerable<RideDistanceDao>> GetWeekDistance(IEnumerable<string> memberIDs);

        /// <summary>
        /// 取得組隊騎乘路線
        /// </summary>
        /// <param name="rideID">rideID</param>
        /// <param name="index">index</param>
        /// <returns>RideRouteDao</returns>
        Task<RideRouteDao> GetRideRoute(string rideID, int index);
    }
}