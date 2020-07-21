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
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="db">db</param>
        /// <returns>bool</returns>
        Task<bool> DeleteCache(string cacheKey, int? db = null);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>T</returns>
        Task<T> GetCache<T>(string cacheKey, int? db = null);

        /// <summary>
        /// 讀取多筆快取資料
        /// </summary>
        /// <param name="cacheKeys">cacheKeys</param>
        /// <returns>T Map</returns>
        Task<Dictionary<string, T>> GetCache<T>(IEnumerable<string> cacheKeys, int? db = null);

        /// <summary>
        /// 讀取快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <returns>T</returns>
        Task<T> GetCache<T>(string cacheKey, string hashKey, int? db = null);

        /// <summary>
        /// 取得 RedisKeys
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>RedisKeys</returns>
        IEnumerable<string> GetRedisKeys(string cacheKey, int? db = null);

        /// <summary>
        /// 檢查資料是否存在
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="isFuzzy">isFuzzy</param>
        /// <returns>bool</returns>
        Task<bool> IsExist(string cacheKey, bool isFuzzy, int? db = null);

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        Task SetCache(string cacheKey, string dataJSON, TimeSpan? cacheTimes, int? db = null);

        /// <summary>
        /// 寫入快取資料
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="hashKey">hashKey</param>
        /// <param name="dataJSON">dataJSON</param>
        /// <param name="cacheTimes">cacheTimes</param>
        Task SetCache(string cacheKey, string hashKey, string dataJSON, TimeSpan? cacheTimes, int? db = null);

        /// <summary>
        /// 更新快取資料到期時間
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="cacheTimes">cacheTimes</param>
        /// <returns>bool</returns>
        Task<bool> UpdateCacheExpire(string cacheKey, TimeSpan? cacheTimes, int? db = null);
    }
}