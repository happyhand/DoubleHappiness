using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces
{
    /// <summary>
    /// 互動資料庫
    /// </summary>
    public interface IInteractiveRepository
    {
        /// <summary>
        /// 取得被加入好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<string>> GetBeFriendList(string memberID);

        /// <summary>
        /// 取得黑名單列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<string>> GetBlackList(string memberID);

        /// <summary>
        /// 取得好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<string>> GetFriendList(string memberID);
    }
}