using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Member.View;
using DataInfo.Core.Models.Dto.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> KeepOnline(string memberID);

        /// <summary>
        /// 會員登入
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Login(MemberLoginContent content);

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Register(MemberRegisterContent content, string fbToken, string googleToken);

        /// <summary>
        /// 會員重新登入
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Relogin(string memberID);

        #endregion 註冊 \ 登入 \ 登出 \ 保持在線

        #region 會員資料

        /// <summary>
        /// 會員編輯資訊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> EditInfo(MemberEditInfoContent content, string memberID);

        /// <summary>
        /// 搜尋會員
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <param name="searchType">searchType</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Search(string searchKey, int searchType, string searchMemberID);

        /// <summary>
        /// 取得會員名片資訊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetCardInfo(MemberCardInfoContent content, string searchMemberID = null);

        /// <summary>
        /// 取得首頁資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> HomeInfo(string memberID);

        /// <summary>
        /// 會員手機綁定
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="email">email</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> MobileBind(MemberMobileBindContent content, string memberID, string email);

        /// <summary>
        /// 重置會員密碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> ResetPassword(MemberForgetPasswordContent content);

        /// <summary>
        /// 發送會員忘記密碼驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> SendForgetPasswordVerifierCode(MemberRequestForgetPasswordContent content);

        /// <summary>
        /// 發送會員手機綁定驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="email">email</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> SendMobileBindVerifierCode(MemberRequestMobileBindContent content, string memberID, string email);

        /// <summary>
        /// 取得會員詳細資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetDetail(string memberID);

        /// <summary>
        /// 轉換為會員詳細資訊可視資料
        /// </summary>
        /// <param name="memberDaos">memberDaos</param>
        /// <returns>MemberDetailInfoViews</returns>
        Task<IEnumerable<MemberDetailInfoView>> TransformMemberDetailInfoView(IEnumerable<MemberDao> memberDaos);

        /// <summary>
        /// 轉換為會員簡易資訊可視資料
        /// </summary>
        /// <param name="memberDaos">memberDaos</param>
        /// <returns>MemberSimpleInfoViews</returns>
        Task<IEnumerable<MemberSimpleInfoView>> TransformMemberSimpleInfoView(IEnumerable<MemberDao> memberDaos);

        /// <summary>
        /// 會員更新密碼
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="memberID">memberID</param>
        /// <param name="isIgnoreOldPassword">isIgnoreOldPassword</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UpdatePassword(MemberUpdatePasswordContent content, string memberID, bool isIgnoreOldPassword);

        #endregion 會員資料
    }
}