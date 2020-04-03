using DataInfo.Core.Enums;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using DataInfo.Repository.Models.Member;
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
        /// 取得互動資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="isCreator">isCreator</param>
        /// <returns>InteractiveModel</returns>
        public async Task<InteractiveModel> Get(string memberID, bool isCreator)
        {
            try
            {
                if (isCreator)
                {
                    return await this.Db.Queryable<InteractiveModel>().Where(data => data.CreatorID.Equals(memberID)).SingleAsync();
                }
                else
                {
                    return await this.Db.Queryable<InteractiveModel>().Where(data => data.TargetID.Equals(memberID)).SingleAsync();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得互動資料發生錯誤", $"MemberID: {memberID} IsCreator: {isCreator}", ex);
                return null;
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
                return null;
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
                return null;
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