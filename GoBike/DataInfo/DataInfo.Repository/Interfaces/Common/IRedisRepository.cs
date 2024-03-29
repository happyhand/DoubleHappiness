﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interfaces.Common
{
    /// <summary>
    /// Redis 資料庫
    /// </summary>
    public interface IRedisRepository
    {
        /// <summary>
        /// 刪除快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>bool</returns>
        Task<bool> DeleteCache(int db, string cacheKey);

        /// <summary>
        /// 刪除多筆快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKeys">cacheKeys</param>
        /// <returns>bool</returns>
        Task<bool> DeleteCache(int db, IEnumerable<string> cacheKeys);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>T</returns>
        Task<T> GetCache<T>(int db, string cacheKey);

        /// <summary>
        /// 讀取多筆快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKeys">cacheKeys</param>
        /// <returns>T Map</returns>
        Task<Dictionary<string, T>> GetCache<T>(int db, IEnumerable<string> cacheKeys);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>T</returns>
        Task<T> GetHashCache<T>(int db, string cacheKey, string hashKey);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>Dictionary Map</returns>
        Task<Dictionary<RedisValue, RedisValue>> GetHashCache(int db, string cacheKey);

        /// <summary>
        /// 取得 RedisKeys
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisKeys</returns>
        IEnumerable<string> GetRedisKeys(int db, string cacheKey);

        /// <summary>
        /// 檢查資料是否存在
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>bool</returns>
        Task<bool> IsExist(int db, string cacheKey, bool isFuzzy);

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        Task SetCache(int db, string cacheKey, string dataJSON, TimeSpan? cacheTimes);

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        Task SetCache(int db, string cacheKey, string hashKey, string dataJSON, TimeSpan? cacheTimes);

        /// <summary>
        /// 更新快取資料到期時間
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        Task<bool> UpdateCacheExpire(int db, string cacheKey, TimeSpan? cacheTimes);
    }
}