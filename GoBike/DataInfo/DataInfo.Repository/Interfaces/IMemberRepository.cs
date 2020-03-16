using System.Collections.Generic;
using System.Threading.Tasks;
using DataInfo.Repository.Models.Member;

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
        /// 取得會員資料 (By Email)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>MemberModel</returns>
        Task<MemberModel> GetByEmail(string email);

        /// <summary>
        /// 取得會員資料 (模糊查詢)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>MemberModels</returns>
        Task<IEnumerable<MemberModel>> GetByFuzzy(string searchKey);

        /// <summary>
        /// 取得會員資料 (By MemberID)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberModel</returns>
        Task<MemberModel> GetByMemberID(string memberID);

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