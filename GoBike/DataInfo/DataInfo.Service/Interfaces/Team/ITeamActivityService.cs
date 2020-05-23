using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Team.Content;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Team
{
    /// <summary>
    /// 車隊活動服務
    /// </summary>
    public interface ITeamActivityService
    {
        /// <summary>
        /// 新增車隊活動
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> AddActivity(string memberID, TeamAddActivityContent content);

        /// <summary>
        /// 取得車隊活動明細資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetTeamActivityDetail(string memberID, TeamActivityDetailContent content);

        /// <summary>
        /// 取得車隊活動列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetTeamActivityList(string memberID, TeamActivityContent content);
    }
}