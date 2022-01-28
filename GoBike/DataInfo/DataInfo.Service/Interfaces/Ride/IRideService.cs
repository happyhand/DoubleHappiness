using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Ride.Content;
using DataInfo.Core.Models.Enum;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Ride
{
    /// <summary>
    /// 騎乘服務
    /// </summary>
    public interface IRideService
    {
        /// <summary>
        /// 會員新增騎乘資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> AddRideData(AddRideDataContent content, string memberID);

        /// <summary>
        /// 取得好友週里程排名
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetFriendWeekRank(string memberID);

        /// <summary>
        /// 取得騎乘明細記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="rideID">rideID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetRideDetailRecord(string memberID, string rideID);

        /// <summary>
        /// 取得組隊隊員列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetRideGroupMemberList(string memberID);

        /// <summary>
        /// 取得騎乘記錄
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetRideRecord(string memberID);

        /// <summary>
        /// 回覆組隊騎乘
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> ReplyRideGroup(ReplyRideGroupContent content, string memberID);

        /// <summary>
        /// 發送組隊騎乘通知
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> SendNotify(string memberID, RideGroupNotifyContent content);

        /// <summary>
        /// 更新組隊騎乘
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="action">action</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UpdateRideGroup(UpdateRideGroupContent content, string memberID, ActionType action);

        /// <summary>
        /// 更新組隊騎乘座標
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UpdateRideGroupCoordinate(UpdateRideGroupCoordinateContent content, string memberID);

        /// <summary>
        /// 更新組隊騎乘邀請
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UpdateRideGroupInvite(UpdateRideGroupContent content, string memberID);

        /// <summary>
        /// 新增騎乘路線資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> AddRideRouteData(AddRideRouteDataContent content, string memberID);

        /// <summary>
        /// 取得騎乘路線資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="rideID">rideID</param>
        /// <param name="index">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetRideRoute(string memberID, string rideID, int index);
    }
}