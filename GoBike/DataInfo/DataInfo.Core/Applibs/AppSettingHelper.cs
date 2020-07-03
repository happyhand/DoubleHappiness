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
        /// Gets or sets CountyMap
        /// </summary>
        public Dictionary<string, string> CountyMap { get; set; }

        #endregion 縣市對照表

        #region Upload Server 設定資料

        /// <summary>
        /// Gets or sets UploadServer
        /// </summary>
        public UploadServerSetting UploadServer { get; set; }

        /// <summary>
        /// MemberImageSetting
        /// </summary>
        public class MemberImageSetting
        {
            /// <summary>
            /// Gets or sets Path
            /// </summary>
            public string Path { get; set; }
        }

        /// <summary>
        /// RideImageSetting
        /// </summary>
        public class RideImageSetting
        {
            /// <summary>
            /// Gets or sets Path
            /// </summary>
            public string Path { get; set; }
        }

        /// <summary>
        /// TeamActivityImageSetting
        /// </summary>
        public class TeamActivityImageSetting
        {
            /// <summary>
            /// Gets or sets Path
            /// </summary>
            public string Path { get; set; }
        }

        /// <summary>
        /// TeamImageSetting
        /// </summary>
        public class TeamImageSetting
        {
            /// <summary>
            /// Gets or sets Path
            /// </summary>
            public string Path { get; set; }
        }

        /// <summary>
        /// UploadServerSetting
        /// </summary>
        public class UploadServerSetting
        {
            /// <summary>
            /// Gets or sets Api
            /// </summary>
            public string Api { get; set; }

            /// <summary>
            /// Gets or sets Domain
            /// </summary>
            public string Domain { get; set; }

            /// <summary>
            /// Gets or sets ImageFileExtension
            /// </summary>
            public string ImageFileExtension { get; set; }

            /// <summary>
            /// Gets or sets Member
            /// </summary>
            public MemberImageSetting Member { get; set; }

            /// <summary>
            /// Gets or sets Ride
            /// </summary>
            public RideImageSetting Ride { get; set; }

            /// <summary>
            /// Gets or sets Team
            /// </summary>
            public TeamImageSetting Team { get; set; }

            /// <summary>
            /// Gets or sets TeamActivity
            /// </summary>
            public TeamActivityImageSetting TeamActivity { get; set; }
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
            /// Gets or sets Api
            /// </summary>
            public string Api { get; set; }

            /// <summary>
            /// Gets or sets Domain
            /// </summary>
            public string Domain { get; set; }
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

        #region 驗證碼設定資料

        /// <summary>
        /// Gets or sets VerifierCode
        /// </summary>
        public VerifierCodeSetting VerifierCode { get; set; }

        /// <summary>
        /// VerifierCodeSetting
        /// </summary>
        public class VerifierCodeSetting
        {
            /// <summary>
            /// Gets or sets ExpirationDate
            /// </summary>
            public int ExpirationDate { get; set; }

            /// <summary>
            /// Gets or sets Length
            /// </summary>
            public int Length { get; set; }
        }

        #endregion 驗證碼設定資料

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
        /// RedisFlag
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
        /// RedisSubFlag
        /// </summary>
        public class RedisSubFlag
        {
            /// <summary>
            /// Gets or sets LastLogin
            /// </summary>
            public string LastLogin { get; set; }

            /// <summary>
            /// Gets or sets HomeInfo
            /// </summary>
            public string HomeInfo { get; set; }

            /// <summary>
            /// Gets or sets CardInfo
            /// </summary>
            public string CardInfo { get; set; }

            /// <summary>
            /// Gets or sets RideRecord
            /// </summary>
            public string RideRecord { get; set; }
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
            /// Gets or sets ExpirationDate
            /// </summary>
            public int ExpirationDate { get; set; }

            /// <summary>
            /// Gets or sets Flag
            /// </summary>
            public RedisFlag Flag { get; set; }

            /// <summary>
            /// Gets or sets SubFlag
            /// </summary>
            public RedisSubFlag SubFlag { get; set; }
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

        #region Command Server 設定資料

        /// <summary>
        /// Gets or sets CommandType
        /// </summary>
        public CommandServerSetting CommandServer { get; set; }

        /// <summary>
        /// CommandServerSetting
        /// </summary>
        public class CommandServerSetting
        {
            /// <summary>
            /// Gets or sets Ride
            /// </summary>
            public RideServerSetting Ride { get; set; }

            /// <summary>
            /// Gets or sets Team
            /// </summary>
            public TeamServerSetting Team { get; set; }

            /// <summary>
            /// Gets or sets TimeOut
            /// </summary>
            public int TimeOut { get; set; }

            /// <summary>
            /// Gets or sets User
            /// </summary>
            public UserServerSetting User { get; set; }
        }

        /// <summary>
        /// RideServerSetting
        /// </summary>
        public class RideServerSetting
        {
            /// <summary>
            /// Gets or sets ConnectionStrings
            /// </summary>
            public string ConnectionStrings { get; set; }
        }

        /// <summary>
        /// TeamServerSetting
        /// </summary>
        public class TeamServerSetting
        {
            /// <summary>
            /// Gets or sets ConnectionStrings
            /// </summary>
            public string ConnectionStrings { get; set; }
        }

        /// <summary>
        /// UserServerSetting
        /// </summary>
        public class UserServerSetting
        {
            /// <summary>
            /// Gets or sets ConnectionStrings
            /// </summary>
            public string ConnectionStrings { get; set; }
        }

        #endregion Command Server 設定資料

        #region 規則設定資料

        /// <summary>
        /// Gets or sets RuleSetting
        /// </summary>
        public RuleSetting Rule { get; set; }

        /// <summary>
        /// RuleSetting
        /// </summary>
        public class RuleSetting
        {
            /// <summary>
            /// Gets or sets NicknameLength
            /// </summary>
            public int NicknameLength { get; set; }

            /// <summary>
            /// Gets or sets DaysOfNewCreation
            /// </summary>
            public int DaysOfNewCreation { get; set; }

            /// <summary>
            /// Gets or sets TakeBrowseCount
            /// </summary>
            public int TakeBrowseCount { get; set; }
        }

        #endregion 規則設定資料
    }
}