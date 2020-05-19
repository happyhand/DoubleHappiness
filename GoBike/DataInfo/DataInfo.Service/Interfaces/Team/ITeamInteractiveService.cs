using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Team.Content;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Team
{
    /// <summary>
    /// 車隊互動服務
    /// </summary>
    public interface ITeamInteractiveService
    {
        /// <summary>
        /// 申請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> ApplyJoinTeam(string memberID, TeamApplyJoinContent content);

        /// <summary>
        /// 取消申請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> CancelApplyJoinTeam(string memberID, TeamApplyJoinContent content);

        /// <summary>
        /// 取消邀請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> CancelInviteJoinTeam(string memberID, TeamInviteJoinContent content);

        /// <summary>
        /// 邀請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> InviteJoinTeam(string memberID, TeamInviteJoinContent content);

        /// <summary>
        /// 回覆申請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> ResponseApplyJoinTeam(string memberID, TeamResponseApplyJoinContent content);

        /// <summary>
        /// 回覆邀請加入車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> ResponseInviteJoinTeam(string memberID, TeamResponseInviteJoinContent content);
    }
}