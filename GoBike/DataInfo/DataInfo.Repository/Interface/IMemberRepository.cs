using DataInfo.Repository.Models.Data.Member;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interface
{
    /// <summary>
    /// 會員資料庫
    /// </summary>
    public interface IMemberRepository
    {
        #region New

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
        /// 取得會員資料 (By FBToken)
        /// </summary>
        /// <param name="fbToken">fbToken</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByFBToken(string fbToken);

        /// <summary>
        /// 取得會員資料 (By GoogleToken)
        /// </summary>
        /// <param name="googleToken">googleToken</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByGoogleToken(string googleToken);

        /// <summary>
        /// 取得會員資料 (By MemberID)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByMemberID(string memberID);

        #endregion New

        /// <summary>
        /// 取得會員資料列表
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>MemberDatas</returns>
        Task<IEnumerable<MemberData>> GetMemberDataList(IEnumerable<string> memberIDs);

        /// <summary>
        /// 更新會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        Task<bool> UpdateMemberData(MemberData memberData);

        /// <summary>
        /// 驗證會員資料
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>bool</returns>
        Task<bool> VerifyMemberList(IEnumerable<string> memberIDs);
    }
}