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
        #region 代理商

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

        #endregion 代理商

        #region 會員

        #region test

        /// <summary>
        /// 新增會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        void AddMember(MemberData memberData);

        #endregion test

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="accountName">accountName</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberByAccountName(string accountName);

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberByMemberID(int memberID);

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="mobile">mobile</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberByMobile(string mobile);

        /// <summary>
        /// 取得所有會員資料
        /// </summary>
        /// <returns>MemberDatas</returns>
        Task<IEnumerable<MemberData>> GetMemberList();

        #endregion 會員
    }
}