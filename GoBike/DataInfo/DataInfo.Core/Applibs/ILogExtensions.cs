using System;
using Microsoft.Extensions.Logging;

namespace DataInfo.Core.Applibs
{
    /// <summary>
    /// ILog 擴充方法
    /// </summary>
    public static class ILogExtensions
    {
        /// <summary>
        /// LogError
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="logger">logger</param>
        /// <param name="targetType">targetType</param>
        /// <param name="depiction">depiction</param>
        /// <param name="message">message</param>
        /// <param name="ex">ex</param>
        public static void LogError<T>(this ILogger logger, T targetType, string depiction, string message, Exception ex)
        {
            logger.LogError($"【{targetType.GetType().Name}】::{depiction} >>> {message}\n{ex}");
        }

        /// <summary>
        /// LogError
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="logger">logger</param>
        /// <param name="targetType">targetType</param>
        /// <param name="message">message</param>
        /// <param name="ex">ex</param>
        public static void LogError<T>(this ILogger logger, T targetType, string message, Exception ex)
        {
            logger.LogError($"【{targetType.GetType().Name}】>>> {message}\n{ex}");
        }

        /// <summary>
        /// LogInformation
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="logger">logger</param>
        /// <param name="targetType">targetType</param>
        /// <param name="depiction">depiction</param>
        /// <param name="message">message</param>
        public static void LogInformation<T>(this ILogger logger, T targetType, string depiction, string message)
        {
            logger.LogInformation($"【{targetType.GetType().Name}】::{depiction} >>> {message}");
        }

        /// <summary>
        /// LogInformation
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="logger">logger</param>
        /// <param name="targetType">targetType</param>
        /// <param name="message">message</param>
        public static void LogInformation<T>(this ILogger logger, T targetType, string message)
        {
            logger.LogInformation($"【{targetType.GetType().Name}】>>> {message}");
        }

        /// <summary>
        /// LogWarning
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="logger">logger</param>
        /// <param name="targetType">targetType</param>
        /// <param name="depiction">depiction</param>
        /// <param name="message">message</param>
        public static void LogWarning<T>(this ILogger logger, T targetType, string depiction, string message)
        {
            logger.LogWarning($"【{targetType.GetType().Name}】::{depiction} >>> {message}");
        }

        /// <summary>
        /// LogWarning
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="logger">logger</param>
        /// <param name="targetType">targetType</param>
        /// <param name="message">message</param>
        public static void LogWarning<T>(this ILogger logger, T targetType, string message)
        {
            logger.LogWarning($"【{targetType.GetType().Name}】>>> {message}");
        }
    }
}