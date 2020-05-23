using DataInfo.Core.Models.Dao.Member;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces.Member
{
    /// <summary>
    /// 會員資料庫
    /// </summary>
    public interface IMemberRepository
    {
        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberDaos</returns>
        Task<MemberDao> Get(string memberID);

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <param name="ignoreMemberIDs">ignoreMemberIDs</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<MemberDao>> Get(string searchKey, bool isFuzzy, IEnumerable<string> ignoreMemberIDs);

        /// <summary>
        /// 取得會員資料列表
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <param name="ignoreMemberIDs">ignoreMemberIDs</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<MemberDao>> Get(IEnumerable<string> memberIDs, IEnumerable<string> ignoreMemberIDs);

        /// <summary>
        /// 取得會員的在線狀態
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>int</returns>
        Task<int> GetOnlineType(string memberID);
    }
}