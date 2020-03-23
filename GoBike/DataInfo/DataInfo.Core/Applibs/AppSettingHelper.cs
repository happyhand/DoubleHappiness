using System.Collections.Generic;

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

        #region 縣市對照表

        /// <summary>
        /// Gets or sets CountryMap
        /// </summary>
        public Dictionary<string, string> CountryMap { get; set; }

        #endregion 縣市對照表

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

        #region Smtp Server 設定資料

        /// <summary>
        /// Gets or sets SmtpServer
        /// </summary>
        public SmtpServerSetting SmtpServer { get; set; }

        /// <summary>
        /// ServiceDomainSetting
        /// </summary>
        public class SmtpServerSetting
        {
            /// <summary>
            /// Gets or sets Domain
            /// </summary>
            public string Domain { get; set; }

            /// <summary>
            /// Gets or sets SendEmailApi
            /// </summary>
            public string SendEmailApi { get; set; }
        }

        #endregion Smtp Server 設定資料

        #region JWT 設定資料

        /// <summary>
        /// Gets or sets Jwt
        /// </summary>
        public JwtSetting Jwt { get; set; }

        /// <summary>
        /// JwtSetting
        /// </summary>
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

        /// <summary>
        /// AesSetting
        /// </summary>
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

        #region 驗證碼時間設定資料

        /// <summary>
        /// Gets or sets VerifierCodeExpirationDate
        /// </summary>
        public int VerifierCodeExpirationDate { get; set; }

        #endregion 驗證碼時間設定資料

        #region 第三方平台設定資料

        /// <summary>
        /// Gets or sets Platform
        /// </summary>
        public PlatformSetting Platform { get; set; }

        /// <summary>
        /// PlatformFlag
        /// </summary>
        public class PlatformFlag
        {
            /// <summary>
            /// Gets or sets FB
            /// </summary>
            public string FB { get; set; }

            /// <summary>
            /// Gets or sets Google
            /// </summary>
            public string Google { get; set; }
        }

        /// <summary>
        /// PlatformSetting
        /// </summary>
        public class PlatformSetting
        {
            /// <summary>
            /// Gets or sets Flag
            /// </summary>
            public PlatformFlag Flag { get; set; }
        }

        #endregion 第三方平台設定資料

        #region Redis 設定資料

        /// <summary>
        /// Gets or sets Redis
        /// </summary>
        public RedisSetting Redis { get; set; }

        /// <summary>
        /// RedisSetting
        /// </summary>
        public class RedisFlag
        {
            /// <summary>
            /// Gets or sets Member
            /// </summary>
            public string Member { get; set; }

            /// <summary>
            /// Gets or sets VerifierCode
            /// </summary>
            public string VerifierCode { get; set; }
        }

        /// <summary>
        /// RedisSetting
        /// </summary>
        public class RedisSetting
        {
            /// <summary>
            /// Gets or sets ConnectionStrings
            /// </summary>
            public string ConnectionStrings { get; set; }

            /// <summary>
            /// Gets or sets DB
            /// </summary>
            public int DB { get; set; }

            /// <summary>
            /// Gets or sets Flag
            /// </summary>
            public RedisFlag Flag { get; set; }
        }

        #endregion Redis 設定資料

        #region Sql 設定資料

        /// <summary>
        /// Gets or sets Sql
        /// </summary>
        public SqlSetting Sql { get; set; }

        /// <summary>
        /// SqlSetting
        /// </summary>
        public class SqlSetting
        {
            /// <summary>
            /// Gets or sets ConnectionStrings
            /// </summary>
            public string ConnectionStrings { get; set; }
        }

        #endregion Sql 設定資料
    }
}