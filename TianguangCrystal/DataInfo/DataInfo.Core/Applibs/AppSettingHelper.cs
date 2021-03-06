﻿namespace DataInfo.Core.Applibs
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

        #region Base 設定資料

        /// <summary>
        /// Gets or sets Mark
        /// </summary>
        public string Mark { get; set; }

        #endregion Base 設定資料

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
            /// Gets or sets UserRegister
            /// </summary>
            public string UserRegister { get; set; }
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