using System.Collections.Generic;

namespace DataInfo.Core.Applibs
{
    /// <summary>
    /// 共用標記
    /// </summary>
    public class CommonFlagHelper
    {
        /// <summary>
        /// CommonFlag
        /// </summary>
        public static CommonFlagHelper CommonFlag;

        /// <summary>
        /// Gets or sets CountryMap
        /// </summary>
        public Dictionary<string, string> CountryMap { get; set; }

        /// <summary>
        /// Gets or sets PlatformFlag
        /// </summary>
        public PlatformSetting PlatformFlag { get; set; }

        /// <summary>
        /// Gets or sets RedisFlag
        /// </summary>
        public RedisSetting RedisFlag { get; set; }

        /// <summary>
        /// 第三方平台共用標記
        /// </summary>
        public class PlatformSetting
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
        /// Redis 共用標記
        /// </summary>
        public class RedisSetting
        {
            /// <summary>
            /// Gets or sets LastLogin
            /// </summary>
            public string LastLogin { get; set; }

            /// <summary>
            /// Gets or sets Member
            /// </summary>
            public string Member { get; set; }

            /// <summary>
            /// Gets or sets VerifierCode
            /// </summary>
            public string VerifierCode { get; set; }
        }
    }
}