using DataInfo.Core.Models.Enum;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using DataInfo.Core.Models.Dao.Member;
using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers
{
    /// <summary>
    /// 互動資料庫
    /// </summary>
    public class InteractiveRepository : MainBase, IInteractiveRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("InteractiveRepository");

        /// <summary>
        /// 建立互動資料
        /// </summary>
        /// <param name="interactiveModel">interactiveModel</param>
        /// <returns>bool</returns>
        public async Task<bool> Create(InteractiveModel interactiveModel)
        {
            try
            {
                bool isSuccess = await this.Db.Insertable(interactiveModel)
                                              .With(SqlWith.HoldLock)
                                              .With(SqlWith.UpdLock)
                                              .ExecuteCommandAsync() > 0;
                this.logger.LogInfo("建立互動資料結果", $"Result: {isSuccess} InteractiveModel: {JsonConvert.SerializeObject(interactiveModel)}", null);
                return isSuccess;
            }
            catch (Exception ex)
            {
                this.logger.LogError("建立互動資料發生錯誤", $"InteractiveModel: {JsonConvert.SerializeObject(interactiveModel)}", ex);
                return false;
            }
        }

        /// <summary>
        /// 取得會員的互動資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="isCreator">isCreator</param>
        /// <returns>InteractiveModel list</returns>
        public async Task<List<InteractiveModel>> Get(string memberID, bool isCreator)
        {
            try
            {
                if (isCreator)
                {
                    return await this.Db.Queryable<InteractiveModel>().Where(data => data.CreatorID.Equals(memberID)).ToListAsync();
                }
                else
                {
                    return await this.Db.Queryable<InteractiveModel>().Where(data => data.TargetID.Equals(memberID)).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員的互動資料列表發生錯誤", $"MemberID: {memberID} IsCreator: {isCreator}", ex);
                return new List<InteractiveModel>();
            }
        }

        /// <summary>
        /// 取得指定的互動資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="targetID">targetID</param>
        /// <returns>InteractiveModel list</returns>
        public async Task<InteractiveModel> Get(string memberID, string targetID)
        {
            try
            {
                return await this.Db.Queryable<InteractiveModel>().Where(data => data.CreatorID.Equals(memberID))
                                                                  .Where(data => data.TargetID.Equals(targetID))
                                                                  .SingleAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得指定的互動資料列表發生錯誤", $"MemberID: {memberID} TargetID: {targetID}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得被加入好友資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>InteractiveModel list</returns>
        public async Task<List<InteractiveModel>> GetBeFriendList(string memberID)
        {
            try
            {
                return await this.Db.Queryable<InteractiveModel>()
                    .Where(data => data.TargetID.Equals(memberID))
                    .Where(data => data.Status.Equals(InteractiveType.Friend))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得被加入好友資料列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<InteractiveModel>();
            }
        }

        /// <summary>
        /// 取得黑名單資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>InteractiveModel list</returns>
        public async Task<List<InteractiveModel>> GetBlackList(string memberID)
        {
            try
            {
                return await this.Db.Queryable<InteractiveModel>()
                    .Where(data => data.CreatorID.Equals(memberID))
                    .Where(data => data.Status.Equals(InteractiveType.Black))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得黑名單資料列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<InteractiveModel>();
            }
        }

        /// <summary>
        /// 取得好友資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>InteractiveModel list</returns>
        public async Task<List<InteractiveModel>> GetFriendList(string memberID)
        {
            try
            {
                return await this.Db.Queryable<InteractiveModel>()
                    .Where(data => data.CreatorID.Equals(memberID))
                    .Where(data => data.Status.Equals(InteractiveType.Friend))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得好友資料列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<InteractiveModel>();
            }
        }

        /// <summary>
        /// 更新互動資料
        /// </summary>
        /// <param name="interactiveModel">interactiveModel</param>
        /// <returns>bool</returns>
        public async Task<bool> Update(InteractiveModel interactiveModel)
        {
            try
            {
                bool isSuccess = await this.Db.Updateable(interactiveModel)
                                              .With(SqlWith.HoldLock)
                                              .With(SqlWith.UpdLock)
                                              .ExecuteCommandAsync() > 0;
                this.logger.LogInfo("更新互動資料結果", $"Result: {isSuccess} InteractiveModel: {JsonConvert.SerializeObject(interactiveModel)}", null);
                return isSuccess;
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新互動資料發生錯誤", $"InteractiveModel: {JsonConvert.SerializeObject(interactiveModel)}", ex);
                return false;
            }
        }
    }
}