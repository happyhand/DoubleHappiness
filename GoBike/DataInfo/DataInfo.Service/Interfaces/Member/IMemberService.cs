using System.Threading.Tasks;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Response;

namespace DataInfo.Service.Interfaces.Member
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
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> KeepOnline(string memberID);

        /// <summary>
        /// 會員登入(一般登入)
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> Login(string email, string password);

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <param name="confirmPassword">confirmPassword</param>
        /// <param name="isValidatePassword">isValidatePassword</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> Register(string email, string password, string confirmPassword, bool isValidatePassword, string fbToken, string googleToken);

        /// <summary>
        /// 會員登入(重新登入)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> Relogin(string memberID);

        #endregion 註冊 \ 登入 \ 登出 \ 保持在線

        #region 會員資料

        /// <summary>
        /// 會員編輯資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> EditInfo(string memberID, MemberEditInfoContent content);

        /// <summary>
        /// 搜尋會員(模糊比對)
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> FuzzySearch(string searchKey, string searchMemberID);

        /// <summary>
        /// 搜尋會員(嚴格比對)
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> StrictSearch(string searchKey, string searchMemberID = null);

        #endregion 會員資料
    }
}