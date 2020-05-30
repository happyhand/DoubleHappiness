using DataInfo.Core.Models.Dao.Team;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces.Team
{
    /// <summary>
    /// 車隊公告資料庫
    /// </summary>
    public interface ITeamBulletinRepository
    {
        /// <summary>
        /// 取得車隊公告資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="teamID">teamID</param>
        /// <returns>TeamBulletinDaos</returns>
        Task<IEnumerable<TeamBulletinDao>> Get(string memberID, string teamID);
    }
}