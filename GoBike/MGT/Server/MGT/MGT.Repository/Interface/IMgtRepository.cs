using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MGT.Repository.Models.Data;

namespace MGT.Repository.Interface
{
    /// <summary>
    /// 後台資料庫服務
    /// </summary>
    public interface IMgtRepository
    {
        /// <summary>
        /// 新增代理商資料
        /// </summary>
        /// <param name="agentData">agentData</param>
        void AddAgent(AgentData agentData);

        /// <summary>
        /// 取得代理商資料
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>AgentData</returns>
        Task<AgentData> GetAgent(long id);

        /// <summary>
        /// 取得代理商資料
        /// </summary>
        /// <param name="account">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns>AgentData</returns>
        Task<AgentData> GetAgent(string account, string password);
    }
}