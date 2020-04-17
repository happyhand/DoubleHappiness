using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Response;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Member
{
    /// <summary>
    /// 互動服務
    /// </summary>
    public interface IInteractiveService
    {
        /// <summary>
        /// 刪除互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> DeleteInteractive(string memberID, InteractiveContent content);

        /// <summary>
        /// 取得黑名單列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> GetBlackList(string memberID);

        /// <summary>
        /// 取得好友列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> GetFriendList(string memberID);

        /// <summary>
        /// 更新互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <param name="status">status</param>
        /// <returns>ResponseResultDto</returns>
        Task<ResponseResultDto> UpdateInteractive(string memberID, InteractiveContent content, int status);
    }
}