namespace DataInfo.Core.Applibs
{
    /// <summary>
    /// 訊息設定資料
    /// </summary>
    public class MessageHelper
    {
        /// <summary>
        /// Appsetting
        /// </summary>
        public static MessageHelper Message;

        #region ResponseMessage

        /// <summary>
        /// Gets or sets ResponseMessage
        /// </summary>
        public ResponseMessageSetting ResponseMessage { get; set; }

        /// <summary>
        /// MemberMessageSetting
        /// </summary>
        public class MemberMessageSetting
        {
            /// <summary>
            /// Gets or sets EmailEmpty
            /// </summary>
            public string EmailEmpty { get; set; }

            /// <summary>
            /// Gets or sets EmailFormatError
            /// </summary>
            public string EmailFormatError { get; set; }

            /// <summary>
            /// Gets or sets EmailNotExist
            /// </summary>
            public string EmailNotExist { get; set; }

            /// <summary>
            /// Gets or sets MemberIDEmpty
            /// </summary>
            public string MemberIDEmpty { get; set; }

            /// <summary>
            /// Gets or sets MobileEmpty
            /// </summary>
            public string MobileEmpty { get; set; }

            /// <summary>
            /// Gets or sets MobileFormatError
            /// </summary>
            public string MobileFormatError { get; set; }

            /// <summary>
            /// Gets or sets PasswordEmpty
            /// </summary>
            public string PasswordEmpty { get; set; }

            /// <summary>
            /// Gets or sets PasswordFormatError
            /// </summary>
            public string PasswordFormatError { get; set; }

            /// <summary>
            /// Gets or sets PasswordNotMatch
            /// </summary>
            public string PasswordNotMatch { get; set; }

            /// <summary>
            /// Gets or sets SearchEmpty
            /// </summary>
            public string SearchEmpty { get; set; }
        }

        /// <summary>
        /// ResponseMessageSetting
        /// </summary>
        public class ResponseMessageSetting
        {
            /// <summary>
            /// Gets or sets Common
            /// </summary>
            public MemberMessageSetting Member { get; set; }

            /// <summary>
            /// Gets or sets Common
            /// </summary>
            public RideMessageSetting Ride { get; set; }

            /// <summary>
            /// Gets or sets Common
            /// </summary>
            public SmtpMessageSetting Smtp { get; set; }

            /// <summary>
            /// Gets or sets Update
            /// </summary>
            public UpdateMessageSetting Update { get; set; }

            /// <summary>
            /// Gets or sets Common
            /// </summary>
            public VerifyCodeMessageSetting VerifyCode { get; set; }
        }

        /// <summary>
        /// RideMessageSetting
        /// </summary>
        public class RideMessageSetting
        {
            /// <summary>
            /// Gets or sets AltitudeEmpty
            /// </summary>
            public string AltitudeEmpty { get; set; }

            /// <summary>
            /// Gets or sets CountyIDEmpty
            /// </summary>
            public string CountyIDEmpty { get; set; }

            /// <summary>
            /// Gets or sets DistanceEmpty
            /// </summary>
            public string DistanceEmpty { get; set; }

            /// <summary>
            /// Gets or sets LevelEmpty
            /// </summary>
            public string LevelEmpty { get; set; }

            /// <summary>
            /// Gets or sets PhotoEmpty
            /// </summary>
            public string PhotoEmpty { get; set; }

            /// <summary>
            /// Gets or sets RouteEmpty
            /// </summary>
            public string RouteEmpty { get; set; }

            /// <summary>
            /// Gets or sets TimeEmpty
            /// </summary>
            public string TimeEmpty { get; set; }
        }

        /// <summary>
        /// SmtpMessageSetting
        /// </summary>
        public class SmtpMessageSetting
        {
            /// <summary>
            /// Gets or sets SendEmailFail
            /// </summary>
            public string SendEmailFail { get; set; }
        }

        /// <summary>
        /// UpdateMessageSetting
        /// </summary>
        public class UpdateMessageSetting
        {
            /// <summary>
            /// Gets or sets Error
            /// </summary>
            public string Error { get; set; }

            /// <summary>
            /// Gets or sets Fail
            /// </summary>
            public string Fail { get; set; }

            /// <summary>
            /// Gets or sets Success
            /// </summary>
            public string Success { get; set; }
        }

        /// <summary>
        /// VerifyCodeMessageSetting
        /// </summary>
        public class VerifyCodeMessageSetting
        {
            /// <summary>
            /// Gets or sets SendVerifyCodeError
            /// </summary>
            public string SendVerifyCodeError { get; set; }

            /// <summary>
            /// Gets or sets SendVerifyCodeFail
            /// </summary>
            public string SendVerifyCodeFail { get; set; }

            /// <summary>
            /// Gets or sets SendVerifyCodeSuccess
            /// </summary>
            public string SendVerifyCodeSuccess { get; set; }

            /// <summary>
            /// Gets or sets VerifyCodeEmpty
            /// </summary>
            public string VerifyCodeEmpty { get; set; }
        }

        #endregion ResponseMessage
    }
}