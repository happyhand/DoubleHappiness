using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Core.Resource
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
        /// Gets or sets PlatformFlag
        /// </summary>
        public PlatformSetting PlatformFlag { get; set; }

        /// <summary>
        /// Gets or sets RedisFlag
        /// </summary>
        public RedisSetting RedisFlag { get; set; }

        /// <summary>
        /// Gets or sets SeparateFlag
        /// </summary>
        public string SeparateFlag { get; set; }

        /// <summary>
        /// Gets or sets SessionFlag
        /// </summary>
        public SessionSetting SessionFlag { get; set; }

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
            /// Gets or sets Session
            /// </summary>
            public string Session { get; set; }

            /// <summary>
            /// Gets or sets VerifierCode
            /// </summary>
            public string VerifierCode { get; set; }
        }

        /// <summary>
        /// Session 共用標記
        /// </summary>
        public class SessionSetting
        {
            /// <summary>
            /// Gets or sets MemberID
            /// </summary>
            public string MemberID { get; set; }
        }
    }
}