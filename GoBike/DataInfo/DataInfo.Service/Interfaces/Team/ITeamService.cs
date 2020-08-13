using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Enum;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Team
{
    /// <summary>
    /// 車隊服務
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        /// 更換車隊隊長
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> ChangeLeader(TeamChangeLeaderContent content, string memberID);

        /// <summary>
        /// 建立車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Create(TeamCreateContent content, string memberID);

        /// <summary>
        /// 解散車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Disband(TeamContent content, string memberID);

        /// <summary>
        /// 更新車隊資料
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Edit(TeamEditContent content, string memberID);

        /// <summary>
        /// 取得瀏覽車隊資訊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetBrowseInfo(TeamBrowseContent content, string memberID);

        /// <summary>
        /// 取得車隊下拉選單
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetDropMenu(string memberID);

        /// <summary>
        /// 取得車隊資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetInfo(string memberID, string teamID);

        /// <summary>
        /// 取得車隊訊息
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetMessage(TeamContent content, string memberID);

        /// <summary>
        /// 取得車隊設定資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetSetting(string memberID, string teamID);

        /// <summary>
        /// 踢離車隊隊員
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Kick(TeamKickContent content, string memberID);

        /// <summary>
        /// 離開車隊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Leave(string memberID, string teamID);

        /// <summary>
        /// 搜尋車隊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Search(TeamSearchContent content, string memberID);

        /// <summary>
        /// 更新車隊副隊長
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="action">action</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UpdateViceLeader(TeamUpdateViceLeaderContent content, string memberID, ActionType action);
    }
}