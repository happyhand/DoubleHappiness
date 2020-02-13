using DataInfo.Repository.Models.Data.Member;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interface.Sql
{
    /// <summary>
    /// 會員資料庫
    /// </summary>
    public interface ISQLMemberRepository
    {
        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        Task<bool> CreateMemberData(MemberData memberData);

        /// <summary>
        /// 取得會員資料 (By Email)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByEmail(string email);

        /// <summary>
        /// 取得會員資料 (By FB)
        /// </summary>
        /// <param name="fbToken">fbToken</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByFB(string fbToken);

        /// <summary>
        /// 取得會員資料 (By Google)
        /// </summary>
        /// <param name="googleToken">googleToken</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByGoogle(string googleToken);

        /// <summary>
        /// 取得會員資料 (By MemberID)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByMemberID(string memberID);

        /// <summary>
        /// 更新會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        Task<bool> UpdateMemberData(MemberData memberData);
    }
}