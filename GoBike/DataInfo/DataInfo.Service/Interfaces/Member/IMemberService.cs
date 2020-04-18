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
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> KeepOnline(string memberID);

        /// <summary>
        /// 會員登入(一般登入)
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> Login(MemberLoginContent content);

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="isValidatePassword">isValidatePassword</param>
        /// <param name="fbToken">fbToken</param>
        /// <param name="googleToken">googleToken</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> Register(MemberRegisterContent content, bool isValidatePassword, string fbToken, string googleToken);

        /// <summary>
        /// 會員登入(重新登入)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> Relogin(string memberID);

        #endregion 註冊 \ 登入 \ 登出 \ 保持在線

        #region 會員資料

        /// <summary>
        /// 會員編輯資訊
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> EditInfo(string memberID, MemberEditInfoContent content);

        /// <summary>
        /// 會員修改密碼
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> EditPassword(string memberID, MemberEditPasswordContent content);

        /// <summary>
        /// 搜尋會員(模糊比對)
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> FuzzySearch(MemberSearchContent content, string searchMemberID);

        /// <summary>
        /// 會員手機綁定
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> MobileBind(string memberID, MemberMobileBindContent content);

        /// <summary>
        /// 重置會員密碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> ResetPassword(MemberForgetPasswordContent content);

        /// <summary>
        /// 發送會員忘記密碼驗證碼
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> SendForgetPasswordVerifierCode(MemberForgetPasswordContent content);

        /// <summary>
        /// 發送會員手機綁定驗證碼
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> SendMobileBindVerifierCode(string email, MemberMobileBindContent content);

        /// <summary>
        /// 搜尋會員(嚴格比對)
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="searchMemberID">searchMemberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResult> StrictSearch(MemberSearchContent content, string searchMemberID = null);

        /// <summary>
        /// 轉換為會員簡易資訊可視資料
        /// </summary>
        /// <param name="ignoreMemberIds">ignoreMemberIds</param>
        /// <param name="memberModels">memberModels</param>
        /// <returns>MemberSimpleInfoViews</returns>
        Task<IEnumerable<MemberSimpleInfoView>> TransformMemberModel(IEnumerable<string> ignoreMemberIds, IEnumerable<MemberModel> memberModels);

        /// <summary>
        /// 轉換為會員簡易資訊可視資料
        /// </summary>
        /// <param name="ignoreMemberIds">ignoreMemberIds</param>
        /// <param name="memberIDs">memberIDs</param>
        /// <returns>MemberSimpleInfoViews</returns>
        Task<IEnumerable<MemberSimpleInfoView>> TransformMemberModel(IEnumerable<string> ignoreMemberIds, IEnumerable<string> memberIDs);

        #endregion 會員資料
    }
}