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
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Add(TeamAddBulletinContent content, string memberID);

        /// <summary>
        /// 刪除車隊公告
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="bulletinID">bulletinID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Delete(string memberID, string bulletinID);

        /// <summary>
        /// 更新車隊公告
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Edit(TeamUpdateBulletinContent content, string memberID);

        /// <summary>
        /// 取得車隊公告列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetList(string memberID, string teamID);
    }
}