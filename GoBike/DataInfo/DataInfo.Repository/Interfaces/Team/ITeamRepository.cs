using DataInfo.Core.Models.Dao.Member;
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
        /// 取得車隊成員列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<MemberDao>> GetMemberList(string memberID, string teamID);

        /// <summary>
        /// 取得申請加入會員列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<MemberDao>> GetMemberOfApplyJoin(string memberID, string teamID);

        /// <summary>
        /// 取得附近車隊資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="county">county</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetNearbyTeam(string memberID, int county);

        /// <summary>
        /// 取得新創車隊資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetNewCreationTeam(string memberID);

        /// <summary>
        /// 取得推薦車隊資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetRecommendTeam(string memberID);

        /// <summary>
        /// 取得申請車隊列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> GetTeamOfApplyJoin(string memberID);

        /// <summary>
        /// 搜尋車隊
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> Search(string key);

        /// <summary>
        /// 是否有車隊隊長身分
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>bool</returns>
        Task<bool> HasTeamLeaderRole(string memberID);
    }
}