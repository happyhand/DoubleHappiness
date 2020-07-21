using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces.Common;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using NLog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers.Common
{
    /// <summary>
    /// Redis 資料庫
    /// </summary>
    public class RedisRepository : IRedisRepository
    {
        /// <summary>
        /// connectionMultiplexer
        /// </summary>
        private readonly ConnectionMultiplexer connectionMultiplexer;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("RedisRepository");

        /// <summary>
        /// redisServer
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

            this.connectionMultiplexer = lazyConnection.Value;
            this.redisServer = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
        }

        /// <summary>
        /// 取得 database
        /// </summary>
        /// <param name="db">db</param>
        /// <returns>IDatabase</returns>
        private IDatabase GetDatabase(int db)
        {
            return this.connectionMultiplexer.GetDatabase(db);
        }

        /// <summary>
        /// 刪除快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="db">db</param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteCache(string cacheKey, int? db = null)
        {
            try
            {
                IDatabase database = db.HasValue ? this.GetDatabase(db.Value) : this.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
                return await database.KeyDeleteAsync(cacheKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除快取資料發生錯誤", $"CacheKey: {cacheKey} DB: {db}", ex);
                return false;
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>T</returns>
        public async Task<T> GetCache<T>(string cacheKey, int? db = null)
        {
            try
            {
                IDatabase database = db.HasValue ? this.GetDatabase(db.Value) : this.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
                string dataJson = await database.StringGetAsync(cacheKey).ConfigureAwait(false);
                return string.IsNullOrEmpty(dataJson) ? default : JsonConvert.DeserializeObject<T>(dataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey} DB: {db}", ex);
                return default;
            }
        }

        /// <summary>
        /// 讀取多筆快取資料
        /// </summary>
        /// <param name="cacheKeys">cacheKeys</param>
        /// <returns>T Map</returns>
        public async Task<Dictionary<string, T>> GetCache<T>(IEnumerable<string> cacheKeys, int? db = null)
        {
            try
            {
                RedisKey[] redisKeys = cacheKeys.Select(key => (RedisKey)key).ToArray();
                IDatabase database = db.HasValue ? this.GetDatabase(db.Value) : this.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
                RedisValue[] redisValues = await database.StringGetAsync(redisKeys).ConfigureAwait(false);
                IEnumerable<T> datas = redisValues.Select(redisValue => string.IsNullOrEmpty(redisValue) ? default : JsonConvert.DeserializeObject<T>(redisValue));
                return cacheKeys.Select((key, index) => new { key, index }).ToDictionary(data => data.key, data => datas.ElementAtOrDefault(data.index));
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取多筆快取資料發生錯誤", $"CacheKeys: {JsonConvert.SerializeObject(cacheKeys)} DB: {db}", ex);
                return new Dictionary<string, T>();
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>T</returns>
        public async Task<T> GetCache<T>(string cacheKey, string hashKey, int? db = null)
        {
            try
            {
                IDatabase database = db.HasValue ? this.GetDatabase(db.Value) : this.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
                string dataJson = await database.HashGetAsync(cacheKey, hashKey).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(dataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey} HashKey: {hashKey} DB: {db}", ex);
                return default;
            }
        }

        /// <summary>
        /// 取得 RedisKeys
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisKeys</returns>
        public IEnumerable<string> GetRedisKeys(string cacheKey, int? db = null)
        {
            return this.redisServer.Keys(db.HasValue ? db.Value : AppSettingHelper.Appsetting.Redis.DB, pattern: cacheKey)
                                   .Select(key => key.ToString());
        }

        /// <summary>
        /// 檢查資料是否存在
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>bool</returns>
        public async Task<bool> IsExist(string cacheKey, bool isFuzzy, int? db = null)
        {
            try
            {
                if (isFuzzy)
                {
                    return this.GetRedisKeys(cacheKey).Any();
                }
                else
                {
                    IDatabase database = db.HasValue ? this.GetDatabase(db.Value) : this.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
                    return await database.KeyExistsAsync(cacheKey).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("檢查資料是否存在發生錯誤", $"CacheKey: {cacheKey} IsFuzzy: {isFuzzy} DB: {db}", ex);
                return false;
            }
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        public async Task SetCache(string cacheKey, string dataJSON, TimeSpan? cacheTimes, int? db = null)
        {
            try
            {
                IDatabase database = db.HasValue ? this.GetDatabase(db.Value) : this.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
                bool isSuccess = await database.StringSetAsync(cacheKey, dataJSON, cacheTimes).ConfigureAwait(false);
                this.logger.LogInfo("寫入快取資料結果", $"Result: {isSuccess} CacheKey: {cacheKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)} DB: {db}", null);
            }
            catch (Exception ex)
            {
                this.logger.LogError("寫入快取資料發生錯誤", $"CacheKey: {cacheKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)} DB: {db}", ex);
            }
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        public async Task SetCache(string cacheKey, string hashKey, string dataJSON, TimeSpan? cacheTimes, int? db = null)
        {
            try
            {
                IDatabase database = db.HasValue ? this.GetDatabase(db.Value) : this.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
                bool isSuccess = await database.HashSetAsync(cacheKey, hashKey, dataJSON).ConfigureAwait(false);
                this.UpdateCacheExpire(cacheKey, cacheTimes);
                this.logger.LogInfo("寫入快取資料結果", $"Result: {isSuccess} CacheKey: {cacheKey} HashKey: {hashKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)} DB: {db}", null);
            }
            catch (Exception ex)
            {
                this.logger.LogError("寫入快取資料發生錯誤", $"CacheKey: {cacheKey} HashKey: {hashKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)} DB: {db}", ex);
            }
        }

        /// <summary>
        /// 更新快取資料到期時間
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateCacheExpire(string cacheKey, TimeSpan? cacheTimes, int? db = null)
        {
            try
            {
                IDatabase database = db.HasValue ? this.GetDatabase(db.Value) : this.GetDatabase(AppSettingHelper.Appsetting.Redis.DB);
                return await database.KeyExpireAsync(cacheKey, cacheTimes).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新快取資料到期時間發生錯誤", $"CacheKey: {cacheKey} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)} DB: {db}", ex);
                return false;
            }
        }
    }
}