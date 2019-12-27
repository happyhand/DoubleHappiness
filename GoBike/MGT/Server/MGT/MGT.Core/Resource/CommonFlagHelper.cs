namespace MGT.Core.Resource
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
            /// Gets or sets Agent
            /// </summary>
            public string Agent { get; set; }
        }
    }
}