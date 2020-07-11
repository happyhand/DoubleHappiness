using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dao.Member.Table;
using DataInfo.Core.Models.Enum;
using SqlSugar;
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
        /// 取得指定會員資料
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="type">type</param>
        /// <returns>MemberDao</returns>
        Task<MemberDao> Get(string key, MemberSearchType type);

        /// <summary>
        /// 搜詢會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <param name="ignoreMemberIDs">ignoreMemberIDs</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<MemberDao>> Search(string key, bool isFuzzy, IEnumerable<string> ignoreMemberIDs);

        /// <summary>
        /// 取得指定會員資料列表
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

        /// <summary>
        /// 轉換 MemberDao
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>MemberDaos</returns>
        Task<IEnumerable<MemberDao>> TransformMemberDao(ISugarQueryable<UserAccount, UserInfo> query);
    }
}