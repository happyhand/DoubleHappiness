using DataInfo.Core.Models.Dao.Team;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces.Team
{
    /// <summary>
    /// 車隊資料庫
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// 取得車隊資料
        /// </summary>
        /// <param name="teamID">teamID</param>
        /// <returns>TeamDao</returns>
        Task<TeamDao> Get(string teamID);

        /// <summary>
        /// 取得車隊資料列表
        /// </summary>
        /// <param name="teamIDs">teamIDs</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> Get(IEnumerable<string> teamIDs);

        /// <summary>
        /// 取得申請車隊列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetApplyTeamList(string memberID);

        /// <summary>
        /// 取得邀請車隊列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetInviteTeamList(string memberID);

        /// <summary>
        /// 取得附近車隊資料列表
        /// </summary>
        /// <param name="county">county</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetNearbyTeam(int county);

        /// <summary>
        /// 取得新創車隊資料列表
        /// </summary>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetNewCreationTeam();

        /// <summary>
        /// 取得推薦車隊資料列表
        /// </summary>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetRecommendTeam();

        /// <summary>
        /// 搜尋車隊
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> Search(string key);
    }
}