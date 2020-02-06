using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DataInfo.Core.Extensions
{
    /// <summary>
    /// Session 擴充方法
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// GetObject
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="session">session</param>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        public static T GetObject<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// SetObject
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="session">session</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}