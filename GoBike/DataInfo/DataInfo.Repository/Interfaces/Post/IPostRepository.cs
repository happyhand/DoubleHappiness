using DataInfo.Core.Models.Dao.Post;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces.Post
{
    /// <summary>
    /// 貼文資料庫
    /// </summary>
    public interface IPostRepository
    {
        /// <summary>
        /// 取得會員的貼文列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>PostInfoDaos</returns>
        Task<IEnumerable<PostInfoDao>> GetMemberPostIDList(string memberID);

        /// <summary>
        /// 取得會員的塗鴉牆貼文列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>PostInfoDaos</returns>
        Task<IEnumerable<PostInfoDao>> GetMemberPostShowList(string memberID);
    }
}