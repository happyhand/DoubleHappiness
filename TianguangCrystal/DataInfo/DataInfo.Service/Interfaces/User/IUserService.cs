using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.User.Content;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.User
{
    /// <summary>
    /// 會員服務
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Login(UserLoginContent content);

        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        Task<ResponseResult> Register(UserRegisterContent content);
    }
}