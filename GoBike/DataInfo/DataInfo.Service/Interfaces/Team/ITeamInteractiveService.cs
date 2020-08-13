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
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> ApplyJoinTeam(TeamApplyJoinContent content, string memberID);

        /// <summary>
        /// 取消申請加入車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> CancelApplyJoinTeam(TeamApplyJoinContent content, string memberID);

        /// <summary>
        /// 回覆申請加入車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> ResponseApplyJoinTeam(TeamResponseApplyJoinContent content, string memberID);
    }
}