using DataInfo.Core.Models.Dao.Member;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces
{
    /// <summary>
    /// 騎乘資料庫
    /// </summary>
    public interface IRideRepository
    {
        /// <summary>
        /// 建立騎乘資料
        /// </summary>
        /// <param name="rideModel">rideModel</param>
        /// <returns>bool</returns>
        Task<bool> Create(RideModel rideModel);

        /// <summary>
        /// 取得騎乘資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideModel</returns>
        Task<RideModel> Get(string rideID);

        /// <summary>
        /// 取得會員的騎乘資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>RideModel list</returns>
        Task<List<RideModel>> GetListOfMember(string memberID);

        /// <summary>
        /// 更新騎乘資料
        /// </summary>
        /// <param name="rideModel">rideModel</param>
        /// <returns>bool</returns>
        Task<bool> Update(RideModel rideModel);
    }
}