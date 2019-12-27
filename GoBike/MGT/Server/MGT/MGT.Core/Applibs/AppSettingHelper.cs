namespace MGT.Core.Applibs
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
            /// Gets or sets DBConnection
            /// </summary>
            public string DBConnection { get; set; }

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
        }

        #endregion Service Domain 設定資料
    }
}