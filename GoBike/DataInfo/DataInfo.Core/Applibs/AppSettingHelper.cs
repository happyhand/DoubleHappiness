using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Core.Applibs
{
    /// <summary>
    /// APP 設定資料
    /// </summary>
    public class AppSettingHelper
    {
        /// <summary>
        /// Appsetting
        /// </summary>
        public static AppSettingHelper Appsetting;

        #region Session 設定資料

        public int SeesionDeadline { get; set; }

        #endregion Session 設定資料

        #region 連線設定資料

        /// <summary>
        /// Gets or sets ConnectionStrings
        /// </summary>
        public ConnectionStringsSetting ConnectionStrings { get; set; }

        /// <summary>
        /// ConnectionStringsSetting
        /// </summary>
        public class ConnectionStringsSetting
        {
            /// <summary>
            /// Gets or sets RedisConnection
            /// </summary>
            public string RedisConnection { get; set; }
        }

        #endregion 連線設定資料

        #region Service Domain 設定資料

        /// <summary>
        /// Gets or sets ServiceDomain
        /// </summary>
        public ServiceDomainSetting ServiceDomain { get; set; }

        /// <summary>
        /// ServiceDomainSetting
        /// </summary>
        public class ServiceDomainSetting
        {
            /// <summary>
            /// Gets or sets SmtpService
            /// </summary>
            public string SmtpService { get; set; }
        }

        #endregion Service Domain 設定資料
    }
}