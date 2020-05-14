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
        /// 會員登入(一般登入)
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Login(MemberLoginContent content);

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="isValidatePassword">isValidatePassword</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Register(MemberRegisterContent content, bool isValidatePassword, string fbToken, string googleToken);

        /// <summary>
        /// 會員登入(重新登入)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Relogin(string memberID);

        #endregion 註冊 \ 登入 \ 登出 \ 保持在線

        #region 會員資料

        /// <summary>
        /// 會員編輯資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> EditInfo(string memberID, MemberEditInfoContent content);

        /// <summary>
        /// 搜尋會員(模糊比對)
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> FuzzySearch(MemberSearchContent content, string searchMemberID);

        /// <summary>
        /// 取得會員名片資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> GetCardInfo(string memberID, string searchMemberID = null);

        /// <summary>
        /// 取得首頁資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> HomeInfo(string memberID);

        /// <summary>
        /// 會員手機綁定
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> MobileBind(string memberID, MemberMobileBindContent content);

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
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> SendMobileBindVerifierCode(string memberID, MemberRequestMobileBindContent content);

        /// <summary>
        /// 搜尋會員(嚴格比對)
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> StrictSearch(MemberSearchContent content, string searchMemberID = null);

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
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <param name="isIgnoreOldPassword">isIgnoreOldPassword</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> UpdatePassword(string memberID, MemberUpdatePasswordContent content, bool isIgnoreOldPassword);

        #endregion 會員資料
    }
}