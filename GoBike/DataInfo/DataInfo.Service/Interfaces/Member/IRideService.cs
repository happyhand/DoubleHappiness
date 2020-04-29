using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Response;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Member
{
    /// <summary>
    /// 騎乘服務
    /// </summary>
    public interface IRideService
    {
        /// <summary>
        /// 會員新增騎乘資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> AddRideData(string memberID, RideInfoContent content);

        /// <summary>
        /// 取得騎乘資料
        /// </summary>
        /// <param name="rideID">rideID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetRideData(string rideID);

        /// <summary>
        /// 取得會員的騎乘資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetRideDataListOfMember(string memberID);
    }
}