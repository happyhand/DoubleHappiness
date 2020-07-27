using DataInfo.Core.Models.Dao.Team;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces.Team
{
    /// <summary>
    /// 車隊活動資料庫
    /// </summary>
    public interface ITeamActivityRepository
    {
        /// <summary>
        /// 取得車隊活動資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamIDs">teamIDs</param>
        /// <returns>TeamActivityDaos</returns>
        Task<IEnumerable<TeamActivityDao>> Get(string memberID, string teamID);

        /// <summary>
        /// 取得會員已加入的車隊活動資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>TeamActivityDaos</returns>
        Task<IEnumerable<TeamActivityDao>> Get(string memberID);

        /// <summary>
        /// 取得車隊活動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <param name="actID">actID</param>
        /// <returns>TeamActivityDao</returns>
        Task<TeamActivityDao> Get(string memberID, string teamID, string actID);
    }
}