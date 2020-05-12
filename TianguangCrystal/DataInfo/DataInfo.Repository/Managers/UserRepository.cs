using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao;
using DataInfo.Core.Models.Dao.Table;
using DataInfo.Core.Models.Dto.User.Request;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers
{
    /// <summary>
    /// 使用者資料庫
    /// </summary>
    public class UserRepository : MainBase, IUserRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("UserRepository");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="redisRepository">redisRepository</param>
        public UserRepository(IMapper mapper, IRedisRepository redisRepository)
        {
            this.mapper = mapper;
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <returns>UserRegisterResultType</returns>
        public async Task<UserRegisterResultType> Create(UserRegisterRequest request)
        {
            try
            {
                bool isRepeatEmail = await this.Db.Queryable<UserData>().Where(data => data.Email.Equals(request.Email)).CountAsync().ConfigureAwait(false) > 0;
                if (isRepeatEmail)
                {
                    return UserRegisterResultType.EmailRepeat;
                }

                bool isRepeatMobile = await this.Db.Queryable<UserData>().Where(data => data.Mobile.Equals(request.Mobile)).CountAsync().ConfigureAwait(false) > 0;
                if (isRepeatMobile)
                {
                    return UserRegisterResultType.MobileRepeat;
                }

                UserData userData = this.mapper.Map<UserData>(request);
                userData.CreateDate = DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss");
                int result = await this.Db.Saveable(userData).ExecuteCommandAsync().ConfigureAwait(false);
                return result > 0 ? UserRegisterResultType.Success : UserRegisterResultType.Fail;
            }
            catch (Exception ex)
            {
                this.logger.LogError("建立使用者發生錯誤", $"Request: {JsonConvert.SerializeObject(request)}", ex);
                return UserRegisterResultType.Fail;
            }
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="searchKey">searchKey</param>
        /// <returns>UserDao</returns>
        public async Task<UserDao> Get(string searchKey)
        {
            try
            {
                UserData userData = null;
                if (Utility.ValidateEmail(searchKey))
                {
                    userData = await this.Db.Queryable<UserData>().Where(data => data.Email.Equals(searchKey)).SingleAsync().ConfigureAwait(false);
                }
                else if (Utility.ValidateMobile(searchKey))
                {
                    userData = await this.Db.Queryable<UserData>().Where(data => data.Mobile.Equals(searchKey)).SingleAsync().ConfigureAwait(false);
                }

                return userData != null ? this.mapper.Map<UserDao>(userData) : null;
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得使用者資料發生錯誤", $"SearchKey: {searchKey}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns>UserDao</returns>
        public async Task<UserDao> Get(int userID)
        {
            try
            {
                UserData userData = await this.Db.Queryable<UserData>().Where(data => data.Id.Equals(userID)).SingleAsync().ConfigureAwait(false);
                return userData != null ? this.mapper.Map<UserDao>(userData) : null;
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得使用者資料發生錯誤", $"UserID: {userID}", ex);
                return null;
            }
        }
    }
}