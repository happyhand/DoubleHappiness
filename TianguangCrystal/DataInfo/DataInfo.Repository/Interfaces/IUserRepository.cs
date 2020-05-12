using DataInfo.Core.Models.Dao;
using DataInfo.Core.Models.Dto.User.Request;
using DataInfo.Core.Models.Enum;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces
{
    /// <summary>
    /// 使用者資料庫
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <returns>UserRegisterResultType</returns>
        Task<UserRegisterResultType> Create(UserRegisterRequest request);

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <returns>UserDao</returns>
        Task<UserDao> Get(string searchKey);

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns>UserDao</returns>
        Task<UserDao> Get(int userID);
    }
}