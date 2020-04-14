using DataInfo.Core.Models.Dao.Member;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces
{
    /// <summary>
    /// 會員資料庫
    /// </summary>
    public interface IMemberRepository
    {
        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="memberModel">memberModel</param>
        /// <returns>bool</returns>
        Task<bool> Create(MemberModel memberModel);

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>MemberModels</returns>
        Task<IEnumerable<MemberModel>> Get(string searchKey, bool isFuzzy);

        /// <summary>
        /// 取得會員資料列表
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>MemberModels</returns>
        Task<IEnumerable<MemberModel>> Get(IEnumerable<string> memberIDs);

        /// <summary>
        /// 更新會員資料
        /// </summary>
        /// <param name="memberModel">memberModel</param>
        /// <returns>bool</returns>
        Task<bool> Update(MemberModel memberModel);

        /// <summary>
        /// 更新多筆會員資料
        /// </summary>
        /// <param name="memberModels">memberModels</param>
        /// <returns>bool</returns>
        Task<bool> Update(List<MemberModel> memberModels);
    }
}