using System.Threading.Tasks;
using DataInfo.Service.Models.Response;
using Microsoft.AspNetCore.Http;

namespace DataInfo.Service.Interface.Member
{
    /// <summary>
    /// 會員服務
    /// </summary>
    public interface IMemberService
    {
        #region 註冊 \ 登入 \ 登出 \ 保持在線

        /// <summary>
        /// 會員保持在線
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="memberID">memberID</param>
        void KeepOnline(ISession session, string memberID);

        /// <summary>
        /// 會員登入
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> Login(ISession session, string email, string password);

        /// <summary>
        /// 會員登出
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="memberID">memberID</param>
        void Logout(ISession session, string memberID);

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <param name="isVerifyPassword">isVerifyPassword</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> Register(string email, string password, bool isVerifyPassword, string fbToken, string googleToken);

        #region TODO

        /// <summary>
        /// 會員登入 (FB)
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="token">token</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> LoginFB(string email, string token);

        /// <summary>
        /// 會員登入 (Google)
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="token">token</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> LoginGoogle(string email, string token);

        #endregion TODO

        #endregion 註冊 \ 登入 \ 登出 \ 保持在線

        #region 會員資料

        /// <summary>
        /// 搜尋會員
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> Search(dynamic searchKey, string searchMemberID);

        #endregion 會員資料
    }
}