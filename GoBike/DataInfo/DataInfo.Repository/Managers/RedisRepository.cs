using DataInfo.Core.Applibs;
using DataInfo.Repository.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers
{
    /// <summary>
    /// Redis 資料庫
    /// </summary>
    public class RedisRepository : IRedisRepository
    {
        /// <summary>
        /// connectionMultiplexer
        /// </summary>
        private readonly IDatabase database;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger<RedisRepository> logger;

        /// <summary>
        /// connectionMultiplexer
        /// </summary>
        private readonly IServer redisServer;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        public RedisRepository(ILogger<RedisRepository> logger)
        {
            this.logger = logger;
            Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(AppSettingHelper.Appsetting.ConnectionStrings.RedisConnection);
            });

            ConnectionMultiplexer connectionMultiplexer = lazyConnection.Value;
            this.database = connectionMultiplexer.GetDatabase(1);
            this.redisServer = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
        }

        /// <summary>
        /// 刪除快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>bool</returns>
        public Task<bool> DeleteCache(string cacheKey)
        {
            try
            {
                return this.database.KeyDeleteAsync(cacheKey);
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "刪除快取資料發生錯誤", $"CacheKey:{cacheKey}", ex);
                return Task.Run(() => { return false; });
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisValue</returns>
        public Task<RedisValue> GetCache(string cacheKey)
        {
            try
            {
                return this.database.StringGetAsync(cacheKey);
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "讀取快取資料發生錯誤", $"CacheKey:{cacheKey}", ex);
                return Task.Run(() => { return RedisValue.EmptyString; });
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>RedisValue</returns>
        public Task<RedisValue> GetCache(string cacheKey, string hashKey)
        {
            try
            {
                return this.database.HashGetAsync(cacheKey, hashKey);
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "讀取快取資料發生錯誤", $"CacheKey:{cacheKey} HashKey:{hashKey}", ex);
                return Task.Run(() => { return RedisValue.EmptyString; });
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>HashEntrys</returns>
        public Task<HashEntry[]> GetHashAllCache(string cacheKey)
        {
            try
            {
                return this.database.HashGetAllAsync(cacheKey);
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "讀取快取資料發生錯誤", $"CacheKey:{cacheKey}", ex);
                return Task.Run(() => { return new HashEntry[] { }; });
            }
        }

        /// <summary>
        /// 取得 RedisKeys
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisKeys</returns>
        public IEnumerable<RedisKey> GetRedisKeys(string cacheKey)
        {
            return this.redisServer.Keys(1, pattern: cacheKey);
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        public Task<bool> SetCache(string cacheKey, string dataJSON, TimeSpan cacheTimes)
        {
            try
            {
                return this.database.StringSetAsync(cacheKey, dataJSON, cacheTimes);
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "寫入快取資料發生錯誤", $"CacheKey:{cacheKey} DataJSON:{dataJSON} CacheTimes:{cacheTimes}", ex);
                return Task.Run(() => { return false; });
            }
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        public Task<bool> SetCache(string cacheKey, string hashKey, string dataJSON, TimeSpan cacheTimes)
        {
            try
            {
                bool keyIsExist = this.database.KeyExistsAsync(cacheKey).Result;
                Task<bool> result = this.database.HashSetAsync(cacheKey, hashKey, dataJSON);
                if (!keyIsExist)
                {
                    this.UpdateCacheExpire(cacheKey, cacheTimes); //// 待測試 hash 方式的暫存時間
                }
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "寫入快取資料發生錯誤", $"CacheKey:{cacheKey} HashKey:{hashKey} DataJSON:{dataJSON} CacheTimes:{cacheTimes}", ex);
                return Task.Run(() => { return false; });
            }
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashFields">hashFields</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        public Task<bool> SetCache(string cacheKey, HashEntry[] hashFields, TimeSpan cacheTimes)
        {
            try
            {
                bool keyIsExist = this.database.KeyExistsAsync(cacheKey).Result;
                this.database.HashSetAsync(cacheKey, hashFields);
                if (!keyIsExist)
                {
                    this.UpdateCacheExpire(cacheKey, cacheTimes); //// 待測試 hash 方式的暫存時間
                }
                return Task.Run(() => { return true; });
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "寫入快取資料發生錯誤", $"CacheKey:{cacheKey} HashEntrys:{JsonConvert.SerializeObject(hashFields)} CacheTimes:{cacheTimes}", ex);
                return Task.Run(() => { return false; });
            }
        }

        /// <summary>
        /// 更新快取資料到期時間
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        public Task<bool> UpdateCacheExpire(string cacheKey, TimeSpan cacheTimes)
        {
            try
            {
                return this.database.KeyExpireAsync(cacheKey, cacheTimes);
            }
            catch (Exception ex)
            {
                this.logger.LogError(this, "更新快取資料到期時間發生錯誤", $"CacheKey:{cacheKey} CacheTimes:{cacheTimes}", ex);
                return Task.Run(() => { return false; });
            }
        }
    }
}