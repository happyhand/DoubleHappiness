using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces
{
    /// <summary>
    /// 車隊資料庫
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// 取得車隊資料列表
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="type">type</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>TeamDaos</returns>
        Task<IEnumerable<TeamDao>> Get(string searchKey, TeamSearchType type, bool isFuzzy);
    }
}