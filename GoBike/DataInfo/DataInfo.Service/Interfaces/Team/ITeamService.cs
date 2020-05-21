using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Team.Content;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Team
{
    /// <summary>
    /// 車隊服務
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        /// 建立車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Create(string memberID, TeamCreateContent content);

        /// <summary>
        /// 取得瀏覽車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetBrowseTeam(string memberID, TeamBrowseContent content);

        /// <summary>
        /// 取得車隊下拉選單
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetTeamDropMenu(string memberID);

        /// <summary>
        /// 取得車隊資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetTeamInfo(string memberID, string teamID);

        /// <summary>
        /// 搜尋車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="searchKey">searchKey</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> SearchTeam(string memberID, string searchKey);
    }
}