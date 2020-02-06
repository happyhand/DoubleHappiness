using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interface
{
    /// <summary>
    /// Redis 資料庫
    /// </summary>
    public interface IRedisRepository
    {
        /// <summary>
        /// 刪除快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>bool</returns>
        Task<bool> DeleteCache(string cacheKey);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisValue</returns>
        Task<RedisValue> GetCache(string cacheKey);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>RedisValue</returns>
        Task<RedisValue> GetCache(string cacheKey, string hashKey);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>RedisValues</returns>
        Task<RedisValue[]> GetCache(string cacheKey, string[] hashKeys);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>HashEntrys</returns>
        Task<HashEntry[]> GetHashAllCache(string cacheKey);

        /// <summary>
        /// 取得 RedisKeys
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisKeys</returns>
        IEnumerable<RedisKey> GetRedisKeys(string cacheKey);

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        void SetCache(string cacheKey, string dataJSON, TimeSpan cacheTimes);

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        void SetCache(string cacheKey, string hashKey, string dataJSON, TimeSpan cacheTimes);

        /// <summary>
        /// 更新快取資料到期時間
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        Task<bool> UpdateCacheExpire(string cacheKey, TimeSpan cacheTimes);
    }
}