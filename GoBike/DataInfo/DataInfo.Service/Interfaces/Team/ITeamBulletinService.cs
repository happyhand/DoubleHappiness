using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Enum;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Team
{
    /// <summary>
    /// 車隊公告服務
    /// </summary>
    public interface ITeamBulletinService
    {
        /// <summary>
        /// 新增車隊公告
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Add(string memberID, TeamAddBulletinContent content);

        /// <summary>
        /// 更新車隊公告
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Edit(string memberID, TeamUpdateBulletinContent content);

        /// <summary>
        /// 取得車隊公告列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetList(string memberID, TeamContent content);
    }
}