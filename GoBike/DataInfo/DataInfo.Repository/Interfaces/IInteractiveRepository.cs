﻿using DataInfo.Repository.Models.Member;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces
{
    /// <summary>
    /// 互動資料庫
    /// </summary>
    public interface IInteractiveRepository
    {
        /// <summary>
        /// 建立互動資料
        /// </summary>
        /// <param name="interactiveModel">interactiveModel</param>
        /// <returns>bool</returns>
        Task<bool> Create(InteractiveModel interactiveModel);

        /// <summary>
        /// 取得互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="isCreator">isCreator</param>
        /// <returns>InteractiveModel</returns>
        Task<InteractiveModel> Get(string memberID, bool isCreator);

        /// <summary>
        /// 取得黑名單資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>InteractiveModel list</returns>
        Task<List<InteractiveModel>> GetBlackList(string memberID);

        /// <summary>
        /// 取得好友資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>InteractiveModel list</returns>
        Task<List<InteractiveModel>> GetFriendList(string memberID);

        /// <summary>
        /// 更新互動資料
        /// </summary>
        /// <param name="interactiveModel">interactiveModel</param>
        /// <returns>bool</returns>
        Task<bool> Update(InteractiveModel interactiveModel);
    }
}