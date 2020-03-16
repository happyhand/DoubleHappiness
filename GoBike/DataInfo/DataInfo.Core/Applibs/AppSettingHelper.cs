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

            /// <summary>
            /// Gets or sets SQLConnection
            /// </summary>
            public string SQLConnection { get; set; }
        }

        #endregion 連線設定資料

        #region Upload Server 設定資料

        /// <summary>
        /// Gets or sets UploadServer
        /// </summary>
        public UploadServerSetting UploadServer { get; set; }

        /// <summary>
        /// ServiceDomainSetting
        /// </summary>
        public class UploadServerSetting
        {
            /// <summary>
            /// Gets or sets Domain
            /// </summary>
            public string Domain { get; set; }

            /// <summary>
            /// Gets or sets ImageApi
            /// </summary>
            public string ImageApi { get; set; }
        }

        #endregion Upload Server 設定資料

        #region JWT 設定資料

        /// <summary>
        /// Gets or sets Jwt
        /// </summary>
        public JwtSetting Jwt { get; set; }

        public class JwtSetting
        {
            /// <summary>
            /// Gets or sets Exp
            /// </summary>
            public int Exp { get; set; }

            /// <summary>
            /// Gets or sets Iss
            /// </summary>
            public string Iss { get; set; }

            /// <summary>
            /// Gets or sets Secret
            /// </summary>
            public string Secret { get; set; }

            /// <summary>
            /// Gets or sets Sub
            /// </summary>
            public string Sub { get; set; }
        }

        #endregion JWT 設定資料

        #region AES 加解密設定資料

        /// <summary>
        /// Gets or sets Aes
        /// </summary>
        public AesSetting Aes { get; set; }

        public class AesSetting
        {
            /// <summary>
            /// Gets or sets Iv
            /// </summary>
            public string Iv { get; set; }

            /// <summary>
            /// Gets or sets Key
            /// </summary>
            public string Key { get; set; }
        }

        #endregion AES 加解密設定資料

        #region KeepOnline 設定資料

        /// <summary>
        /// Gets or sets KeepOnlineTime
        /// </summary>
        public int KeepOnlineTime { get; set; }

        #endregion KeepOnline 設定資料
    }
}