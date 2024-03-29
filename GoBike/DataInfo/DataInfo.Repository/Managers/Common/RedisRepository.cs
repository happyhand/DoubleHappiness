﻿using DataInfo.Core.Applibs;
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
        /// 刪除快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteCache(int db, string cacheKey)
        {
            try
            {
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                return await database.KeyDeleteAsync(cacheKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除快取資料發生錯誤", $"DB: {db} CacheKey: {cacheKey}", ex);
                return false;
            }
        }

        /// <summary>
        /// 刪除多筆快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKeys">cacheKeys</param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteCache(int db, IEnumerable<string> cacheKeys)
        {
            try
            {
                RedisKey[] redisKeys = cacheKeys.Select(key => (RedisKey)key).ToArray();
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                long count = await database.KeyDeleteAsync(redisKeys).ConfigureAwait(false);
                return count == cacheKeys.Count();
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除多筆快取資料發生錯誤", $"DB: {db} CacheKey: {JsonConvert.SerializeObject(cacheKeys)}", ex);
                return false;
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>T</returns>
        public async Task<T> GetCache<T>(int db, string cacheKey)
        {
            try
            {
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                string dataJson = await database.StringGetAsync(cacheKey).ConfigureAwait(false);
                return string.IsNullOrEmpty(dataJson) ? default : JsonConvert.DeserializeObject<T>(dataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"DB: {db} CacheKey: {cacheKey}", ex);
                return default;
            }
        }

        /// <summary>
        /// 讀取多筆快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKeys">cacheKeys</param>
        /// <returns>T Map</returns>
        public async Task<Dictionary<string, T>> GetCache<T>(int db, IEnumerable<string> cacheKeys)
        {
            try
            {
                RedisKey[] redisKeys = cacheKeys.Select(key => (RedisKey)key).ToArray();
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                RedisValue[] redisValues = await database.StringGetAsync(redisKeys).ConfigureAwait(false);
                IEnumerable<T> datas = redisValues.Select(redisValue => string.IsNullOrEmpty(redisValue) ? default : JsonConvert.DeserializeObject<T>(redisValue));
                return cacheKeys.Select((key, index) => new { key, index }).ToDictionary(data => data.key, data => datas.ElementAtOrDefault(data.index));
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取多筆快取資料發生錯誤", $"DB: {db} CacheKeys: {JsonConvert.SerializeObject(cacheKeys)}", ex);
                return new Dictionary<string, T>();
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>T</returns>
        public async Task<T> GetHashCache<T>(int db, string cacheKey, string hashKey)
        {
            try
            {
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                string dataJson = await database.HashGetAsync(cacheKey, hashKey).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(dataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"DB: {db} CacheKey: {cacheKey} HashKey: {hashKey}", ex);
                return default;
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>Dictionary Map</returns>
        public async Task<Dictionary<RedisValue, RedisValue>> GetHashCache(int db, string cacheKey)
        {
            try
            {
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                HashEntry[] data = await database.HashGetAllAsync(cacheKey).ConfigureAwait(false);
                return data.ToDictionary(item => item.Name, item => item.Value);
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"DB: {db} CacheKey: {cacheKey}", ex);
                return default;
            }
        }

        /// <summary>
        /// 取得 RedisKeys
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisKeys</returns>
        public IEnumerable<string> GetRedisKeys(int db, string cacheKey)
        {
            return this.redisServer.Keys(db, pattern: cacheKey).Select(key => key.ToString());
        }

        /// <summary>
        /// 檢查資料是否存在
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>bool</returns>
        public async Task<bool> IsExist(int db, string cacheKey, bool isFuzzy)
        {
            try
            {
                if (isFuzzy)
                {
                    return this.GetRedisKeys(db, cacheKey).Any();
                }
                else
                {
                    IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                    return await database.KeyExistsAsync(cacheKey).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("檢查資料是否存在發生錯誤", $"DB: {db} CacheKey: {cacheKey} IsFuzzy: {isFuzzy}", ex);
                return false;
            }
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        public async Task SetCache(int db, string cacheKey, string dataJSON, TimeSpan? cacheTimes)
        {
            try
            {
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                bool isSuccess = await database.StringSetAsync(cacheKey, dataJSON, cacheTimes).ConfigureAwait(false);
                this.logger.LogInfo("寫入快取資料結果", $"Result: {isSuccess} DB: {db} CacheKey: {cacheKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", null);
            }
            catch (Exception ex)
            {
                this.logger.LogError("寫入快取資料發生錯誤", $"DB: {db} CacheKey: {cacheKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", ex);
            }
        }

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        public async Task SetCache(int db, string cacheKey, string hashKey, string dataJSON, TimeSpan? cacheTimes)
        {
            try
            {
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                bool isSuccess = await database.HashSetAsync(cacheKey, hashKey, dataJSON).ConfigureAwait(false);
                this.UpdateCacheExpire(db, cacheKey, cacheTimes);
                this.logger.LogInfo("寫入快取資料結果", $"Result: {isSuccess} DB: {db} CacheKey: {cacheKey} HashKey: {hashKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", null);
            }
            catch (Exception ex)
            {
                this.logger.LogError("寫入快取資料發生錯誤", $"DB: {db} CacheKey: {cacheKey} HashKey: {hashKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", ex);
            }
        }

        /// <summary>
        /// 更新快取資料到期時間
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateCacheExpire(int db, string cacheKey, TimeSpan? cacheTimes)
        {
            try
            {
                IDatabase database = this.connectionMultiplexer.GetDatabase(db);
                return await database.KeyExpireAsync(cacheKey, cacheTimes).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新快取資料到期時間發生錯誤", $"DB: {db} CacheKey: {cacheKey} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", ex);
                return false;
            }
        }
    }
}