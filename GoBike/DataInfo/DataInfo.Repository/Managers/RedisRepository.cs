using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using Newtonsoft.Json;
using NLog;
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
        private readonly ILogger logger = LogManager.GetLogger("RedisRepository");

        /// <summary>
        /// connectionMultiplexer
        /// </summary>
        private readonly IServer redisServer;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        public RedisRepository()
        {
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
        public async Task<bool> DeleteCache(string cacheKey)
        {
            try
            {
                return await this.database.KeyDeleteAsync(cacheKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除快取資料發生錯誤", $"CacheKey: {cacheKey}", ex);
                return false;
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisValue</returns>
        public async Task<RedisValue> GetCache(string cacheKey)
        {
            try
            {
                return await this.database.StringGetAsync(cacheKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey}", ex);
                return RedisValue.Null;
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>RedisValue</returns>
        public async Task<RedisValue> GetCache(string cacheKey, string hashKey)
        {
            try
            {
                return await this.database.HashGetAsync(cacheKey, hashKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey} HashKey: {hashKey}", ex);
                return RedisValue.Null;
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>RedisValues</returns>
        public async Task<RedisValue[]> GetCache(string cacheKey, string[] hashKeys)
        {
            try
            {
                return await this.database.HashGetAsync(cacheKey, hashKeys.ToRedisValueArray()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey} HashKeys: {JsonConvert.SerializeObject(hashKeys)}", ex);
                return null;
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>HashEntrys</returns>
        public async Task<HashEntry[]> GetHashAllCache(string cacheKey)
        {
            try
            {
                return await this.database.HashGetAllAsync(cacheKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey}", ex);
                return null;
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
        /// 檢查資料是否存在
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>bool</returns>
        public async Task<bool> IsExist(string cacheKey)
        {
            try
            {
                return await this.database.KeyExistsAsync(cacheKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("檢查資料是否存在發生錯誤", $"CacheKey: {cacheKey}", ex);
                return false;
            }
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        public async Task SetCache(string cacheKey, string dataJSON, TimeSpan? cacheTimes)
        {
            try
            {
                bool isSuccess = await this.database.StringSetAsync(cacheKey, dataJSON, cacheTimes).ConfigureAwait(false);
                this.logger.LogInfo("寫入快取資料結果", $"Result: {isSuccess} CacheKey: {cacheKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", null);
            }
            catch (Exception ex)
            {
                this.logger.LogError("寫入快取資料發生錯誤", $"CacheKey: {cacheKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", ex);
            }
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        public async Task SetCache(string cacheKey, string hashKey, string dataJSON, TimeSpan? cacheTimes)
        {
            try
            {
                bool isSuccess = await this.database.HashSetAsync(cacheKey, hashKey, dataJSON).ConfigureAwait(false);
                this.UpdateCacheExpire(cacheKey, cacheTimes);
                this.logger.LogInfo("寫入快取資料結果", $"Result: {isSuccess} CacheKey: {cacheKey} HashKey: {hashKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", null);
            }
            catch (Exception ex)
            {
                this.logger.LogError("寫入快取資料發生錯誤", $"CacheKey: {cacheKey} HashKey: {hashKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", ex);
            }
        }

        /// <summary>
        /// 更新快取資料到期時間
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateCacheExpire(string cacheKey, TimeSpan? cacheTimes)
        {
            try
            {
                return await this.database.KeyExpireAsync(cacheKey, cacheTimes).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新快取資料到期時間發生錯誤", $"CacheKey: {cacheKey} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", ex);
                return false;
            }
        }
    }
}