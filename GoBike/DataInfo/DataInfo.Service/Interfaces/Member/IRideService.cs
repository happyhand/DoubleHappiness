using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Ride.Content;
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
        Task<ResponseResult> AddRideData(string memberID, AddRideInfoContent content);
    }
}