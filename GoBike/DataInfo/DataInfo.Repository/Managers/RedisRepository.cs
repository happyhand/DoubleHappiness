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
        public RedisRepository()
        {
            Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(AppSettingHelper.Appsetting.Redis.ConnectionStrings);
            });

            ConnectionMultiplexer connectionMultiplexer = lazyConnection.Value;
            this.database = connectionMultiplexer.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
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
        /// <returns>T</returns>
        public async Task<T> GetCache<T>(string cacheKey)
        {
            try
            {
                string dataJson = await this.database.StringGetAsync(cacheKey).ConfigureAwait(false);
                this.logger.LogInfo("讀取快取資料", $"dataJson: {dataJson}", null);

                return JsonConvert.DeserializeObject<T>(dataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey}", ex);
                return default;
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>T</returns>
        public async Task<T> GetCache<T>(string cacheKey, string hashKey)
        {
            try
            {
                string dataJson = await this.database.HashGetAsync(cacheKey, hashKey).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(dataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey} HashKey: {hashKey}", ex);
                return default;
            }
        }

        /// <summary>
        /// 取得 RedisKeys
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisKeys</returns>
        public IEnumerable<string> GetRedisKeys(string cacheKey)
        {
            return this.redisServer.Keys(AppSettingHelper.Appsetting.Redis.DB, pattern: cacheKey)
                                   .Select(key => key.ToString());
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