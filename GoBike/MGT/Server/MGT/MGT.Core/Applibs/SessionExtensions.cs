using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MGT.Core.Applibs
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
            string value = session.GetObject<string>(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
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
            session.SetObject(key, JsonConvert.SerializeObject(value));
        }
    }
}