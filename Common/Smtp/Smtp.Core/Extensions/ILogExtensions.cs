using NLog;
using System;

namespace Smtp.Core.Extensions
{
    /// <summary>
    /// ILog 擴充方法
    /// </summary>
    public static class ILogExtensions
    {
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="depiction">depiction</param>
        /// <param name="message">message</param>
        /// <param name="ex">ex</param>
        public static void LogDebug(this ILogger logger, string depiction, string message, Exception ex)
        {
            logger.Debug($"{(string.IsNullOrEmpty(depiction) ? string.Empty : $"{depiction}：")}{message}{(ex == null ? string.Empty : $"\n{ex}")}");
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="depiction">depiction</param>
        /// <param name="message">message</param>
        /// <param name="ex">ex</param>
        public static void LogError(this ILogger logger, string depiction, string message, Exception ex)
        {
            logger.Error($"{(string.IsNullOrEmpty(depiction) ? string.Empty : $"{depiction}：")}{message}{(ex == null ? string.Empty : $"\n{ex}")}");
        }

        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="depiction">depiction</param>
        /// <param name="message">message</param>
        /// <param name="ex">ex</param>
        public static void LogFatal(this ILogger logger, string depiction, string message, Exception ex)
        {
            logger.Fatal($"{(string.IsNullOrEmpty(depiction) ? string.Empty : $"{depiction}：")}{message}{(ex == null ? string.Empty : $"\n{ex}")}");
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="depiction">depiction</param>
        /// <param name="message">message</param>
        /// <param name="ex">ex</param>
        public static void LogInfo(this ILogger logger, string depiction, string message, Exception ex)
        {
            logger.Info($"{(string.IsNullOrEmpty(depiction) ? string.Empty : $"{depiction}：")}{message}{(ex == null ? string.Empty : $"\n{ex}")}");
        }

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="depiction">depiction</param>
        /// <param name="message">message</param>
        /// <param name="ex">ex</param>
        public static void LogWarn(this ILogger logger, string depiction, string message, Exception ex)
        {
            logger.Warn($"{(string.IsNullOrEmpty(depiction) ? string.Empty : $"{depiction}：")}{message}{(ex == null ? string.Empty : $"\n{ex}")}");
        }
    }
}