using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MGT.Repository.Models.Data;

namespace MGT.Service.Interface
{
    /// <summary>
    /// 後台服務
    /// </summary>
    public interface IMgtService
    {
        /// <summary>
        /// 新增代理商資料
        /// </summary>
        /// <returns>Tuple(AgentData, string)</returns>
        void AddAgent(string nickname, string password);

        /// <summary>
        /// 代理商登入
        /// </summary>
        /// <param name="account">account</param>
        /// <param name="password">password</param>
        /// <returns>bool</returns>
        Task<string> AgentLogin(string account, string password);

        /// <summary>
        /// 取得代理商資料
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Tuple(AgentData, string)</returns>
        Task<Tuple<AgentData, string>> GetAgent(long id);
    }
}