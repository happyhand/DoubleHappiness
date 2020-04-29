using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers
{
    /// <summary>
    /// 車隊資料庫
    /// </summary>
    public class TeamRepository : MainBase, ITeamRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("TeamRepository");

        /// <summary>
        /// 取得車隊資料列表
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="type">type</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>TeamDaos</returns>
        public async Task<IEnumerable<TeamDao>> Get(string searchKey, TeamSearchType type, bool isFuzzy)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得車隊資料列表發生錯誤", $"SearchKey: {searchKey} Type: {type} IsFuzzy: {isFuzzy}", ex);
                return new List<TeamDao>();
            }
        }
    }
}