using System.Collections.Generic;
using System.Threading.Tasks;
using DataInfo.Repository.Interface;
using DataInfo.Repository.Models.Data.Member;
using Microsoft.Extensions.Logging;

namespace DataInfo.Repository.Managers
{
    /// <summary>
    /// 會員資料庫
    /// </summary>
    public class MemberRepository : IMemberRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger<MemberRepository> logger;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        public MemberRepository(ILogger<MemberRepository> logger)
        {
            this.logger = logger;
        }

        #region New

        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        public async Task<bool> CreateMemberData(MemberData memberData)
        {
            return true;
        }

        /// <summary>
        /// 取得會員資料 (By Email)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberDataByEmail(string email)
        {
            return null;
        }

        /// <summary>
        /// 取得會員資料 (By FBToken)
        /// </summary>
        /// <param name="fbToken">fbToken</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberDataByFBToken(string fbToken)
        {
            return null;
        }

        /// <summary>
        /// 取得會員資料 (By GoogleToken)
        /// </summary>
        /// <param name="googleToken">googleToken</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberDataByGoogleToken(string googleToken)
        {
            return null;
        }

        /// <summary>
        /// 取得會員資料 (By MemberID)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberDataByMemberID(string memberID)
        {
            return null;
        }

        #endregion New

        /// <summary>
        /// 取得會員資料列表
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>MemberDatas</returns>
        public async Task<IEnumerable<MemberData>> GetMemberDataList(IEnumerable<string> memberIDs)
        {
            return null;
        }

        /// <summary>
        /// 更新會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateMemberData(MemberData memberData)
        {
            return false;
        }

        /// <summary>
        /// 驗證會員資料
        /// </summary>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>bool</returns>
        public async Task<bool> VerifyMemberList(IEnumerable<string> memberIDs)
        {
            return false;
        }
    }
}