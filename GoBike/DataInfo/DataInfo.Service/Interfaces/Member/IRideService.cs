using System.Threading.Tasks;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Response;

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
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> AddRideData(string memberID, RideInfoContent content);

        /// <summary>
        /// 取得騎乘資料
        /// </summary>
        /// <param name="rideID">rideID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> GetRideData(string rideID);

        /// <summary>
        /// 取得會員的騎乘資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> GetRideDataListOfMember(string memberID);

        /// <summary>
        /// 更新騎乘資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> UpdateRideData(string memberID, RideUpdateInfoContent content);
    }
}