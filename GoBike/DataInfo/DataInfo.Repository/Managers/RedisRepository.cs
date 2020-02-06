using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interface;
using Newtonsoft.Json;
using NLog;
using StackExchange.Redis;

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
        public Task<bool> DeleteCache(string cacheKey)
        {
            try
            {
                return this.database.KeyDeleteAsync(cacheKey);
            }
            catch (Exception ex)
            {
                this.logger.LogError("刪除快取資料發生錯誤", $"CacheKey: {cacheKey}", ex);
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
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey}", ex);
                return Task.Run(() => { return RedisValue.Null; });
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
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey} HashKey: {hashKey}", ex);
                return Task.Run(() => { return RedisValue.Null; });
            }
        }

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>RedisValues</returns>
        public Task<RedisValue[]> GetCache(string cacheKey, string[] hashKeys)
        {
            try
            {
                return this.database.HashGetAsync(cacheKey, hashKeys.ToRedisValueArray());
            }
            catch (Exception ex)
            {
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey} HashKeys: {JsonConvert.SerializeObject(hashKeys)}", ex);
                return Task.Run(() => { return new RedisValue[] { }; });
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
                this.logger.LogError("讀取快取資料發生錯誤", $"CacheKey: {cacheKey}", ex);
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
        public void SetCache(string cacheKey, string dataJSON, TimeSpan cacheTimes)
        {
            try
            {
                TaskAwaiter<bool> setCacheAwaiter = this.database.StringSetAsync(cacheKey, dataJSON, cacheTimes).GetAwaiter();
                setCacheAwaiter.OnCompleted(() =>
                {
                    bool isSuccess = setCacheAwaiter.GetResult();
                    this.logger.LogInfo("寫入快取資料結果", $"Result: {isSuccess} CacheKey: {cacheKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", null);
                });
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
        public void SetCache(string cacheKey, string hashKey, string dataJSON, TimeSpan cacheTimes)
        {
            try
            {
                TaskAwaiter<bool> getKeyExistsAwaiter = this.database.KeyExistsAsync(cacheKey).GetAwaiter();
                getKeyExistsAwaiter.OnCompleted(() =>
                {
                    bool keyIsExist = getKeyExistsAwaiter.GetResult();
                    TaskAwaiter<bool> setCacheAwaiter = this.database.HashSetAsync(cacheKey, hashKey, dataJSON).GetAwaiter();
                    setCacheAwaiter.OnCompleted(() =>
                    {
                        bool isSuccess = setCacheAwaiter.GetResult();
                        if (isSuccess)
                        {
                            if (!keyIsExist)
                            {
                                this.UpdateCacheExpire(cacheKey, cacheTimes);
                            }
                        }

                        this.logger.LogInfo("寫入快取資料結果", $"Result: {isSuccess} CacheKey: {cacheKey} HashKey: {hashKey} DataJSON: {dataJSON} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", null);
                    });
                });
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
        public Task<bool> UpdateCacheExpire(string cacheKey, TimeSpan cacheTimes)
        {
            try
            {
                return this.database.KeyExpireAsync(cacheKey, cacheTimes);
            }
            catch (Exception ex)
            {
                this.logger.LogError("更新快取資料到期時間發生錯誤", $"CacheKey: {cacheKey} CacheTimes: {JsonConvert.SerializeObject(cacheTimes)}", ex);
                return Task.Run(() => { return false; });
            }
        }
    }
}