namespace DataInfo.Core.Applibs
{
    /// <summary>
    /// 訊息設定資料
    /// </summary>
    public class MessageHelper
    {
        /// <summary>
        /// Message
        /// </summary>
        public static MessageHelper Message;

        #region ResponseMessage

        /// <summary>
        /// Gets or sets ResponseMessage
        /// </summary>
        public ResponseMessageSetting ResponseMessage { get; set; }

        /// <summary>
        /// AddMessageSetting
        /// </summary>
        public class AddMessageSetting
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
        /// GetMessageSetting
        /// </summary>
        public class GetMessageSetting
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
        /// InteractiveMessageSetting
        /// </summary>
        public class InteractiveMessageSetting
        {
            /// <summary>
            /// Gets or sets TargetError
            /// </summary>
            public string TargetError { get; set; }
        }

        /// <summary>
        /// LoginMessageSetting
        /// </summary>
        public class LoginMessageSetting
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
        /// MemberMessageSetting
        /// </summary>
        public class MemberMessageSetting
        {
            /// <summary>
            /// Gets or sets CountyEmpty
            /// </summary>
            public string CountyEmpty { get; set; }

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
            /// Gets or sets MemberHasBindMobile
            /// </summary>
            public string MemberHasBindMobile { get; set; }

            /// <summary>
            /// Gets or sets MemberIDEmpty
            /// </summary>
            public string MemberIDEmpty { get; set; }

            /// <summary>
            /// Gets or sets MemberNotExist
            /// </summary>
            public string MemberNotExist { get; set; }

            /// <summary>
            /// Gets or sets MobileBind
            /// </summary>
            public string MobileBind { get; set; }

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
            /// Gets or sets PasswordFail
            /// </summary>
            public string PasswordFail { get; set; }

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
        /// RegisterMessageSetting
        /// </summary>
        public class RegisterMessageSetting
        {
            /// <summary>
            /// Gets or sets EmailExist
            /// </summary>
            public string EmailExist { get; set; }

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
        /// ResponseMessageSetting
        /// </summary>
        public class ResponseMessageSetting
        {
            /// <summary>
            /// Gets or sets Add
            /// </summary>
            public AddMessageSetting Add { get; set; }

            /// <summary>
            /// Gets or sets Get
            /// </summary>
            public GetMessageSetting Get { get; set; }

            /// <summary>
            /// Gets or sets Interactive
            /// </summary>
            public InteractiveMessageSetting Interactive { get; set; }

            /// <summary>
            /// Gets or sets Login
            /// </summary>
            public LoginMessageSetting Login { get; set; }

            /// <summary>
            /// Gets or sets Member
            /// </summary>
            public MemberMessageSetting Member { get; set; }

            /// <summary>
            /// Gets or sets Register
            /// </summary>
            public RegisterMessageSetting Register { get; set; }

            /// <summary>
            /// Gets or sets Ride
            /// </summary>
            public RideMessageSetting Ride { get; set; }

            /// <summary>
            /// Gets or sets Smtp
            /// </summary>
            public SmtpMessageSetting Smtp { get; set; }

            /// <summary>
            /// Gets or sets Team
            /// </summary>
            public TeamMessageSetting Team { get; set; }

            /// <summary>
            /// Gets or sets Update
            /// </summary>
            public UpdateMessageSetting Update { get; set; }

            /// <summary>
            /// Gets or sets Upload
            /// </summary>
            public UploadMessageSetting Upload { get; set; }

            /// <summary>
            /// Gets or sets VerifyCode
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
            /// Gets or sets CountyEmpty
            /// </summary>
            public string CountyEmpty { get; set; }

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
        /// TeamMessageSetting
        /// </summary>
        public class TeamMessageSetting
        {
            /// <summary>
            /// Gets or sets ActivityAltitudeEmpty
            /// </summary>
            public string ActivityAltitudeEmpty { get; set; }

            /// <summary>
            /// Gets or sets ActivityDateEmpty
            /// </summary>
            public string ActivityDateEmpty { get; set; }

            /// <summary>
            /// Gets or sets ActivityDistanceEmpty
            /// </summary>
            public string ActivityDistanceEmpty { get; set; }

            /// <summary>
            /// Gets or sets ActivityIDEmpty
            /// </summary>
            public string ActivityIDEmpty { get; set; }

            /// <summary>
            /// Gets or sets ActivityMeetTimeEmpty
            /// </summary>
            public string ActivityMeetTimeEmpty { get; set; }

            /// <summary>
            /// Gets or sets ActivityNotExist
            /// </summary>
            public string ActivityNotExist { get; set; }

            /// <summary>
            /// Gets or sets ActivityPhotoEmpty
            /// </summary>
            public string ActivityPhotoEmpty { get; set; }

            /// <summary>
            /// Gets or sets ActivityTitleEmpty
            /// </summary>
            public string ActivityTitleEmpty { get; set; }

            /// <summary>
            /// Gets or sets AvatarEmpty
            /// </summary>
            public string AvatarEmpty { get; set; }

            /// <summary>
            /// Gets or sets CountyEmpty
            /// </summary>
            public string CountyEmpty { get; set; }

            /// <summary>
            /// Gets or sets ExamineStatusEmpty
            /// </summary>
            public string ExamineStatusEmpty { get; set; }

            /// <summary>
            /// Gets or sets FrontCoverEmpty
            /// </summary>
            public string FrontCoverEmpty { get; set; }

            /// <summary>
            /// Gets or sets ResponseStatusEmpty
            /// </summary>
            public string ResponseStatusEmpty { get; set; }

            /// <summary>
            /// Gets or sets SearchKeyEmpty
            /// </summary>
            public string SearchKeyEmpty { get; set; }

            /// <summary>
            /// Gets or sets SearchStatusEmpty
            /// </summary>
            public string SearchStatusEmpty { get; set; }

            /// <summary>
            /// Gets or sets TeamAuthorityNotEnough
            /// </summary>
            public string TeamAuthorityNotEnough { get; set; }

            /// <summary>
            /// Gets or sets TeamIDEmpty
            /// </summary>
            public string TeamIDEmpty { get; set; }

            /// <summary>
            /// Gets or sets TeamInfoEmpty
            /// </summary>
            public string TeamInfoEmpty { get; set; }

            /// <summary>
            /// Gets or sets TeamNameEmpty
            /// </summary>
            public string TeamNameEmpty { get; set; }

            /// <summary>
            /// Gets or sets TeamNameRepeat
            /// </summary>
            public string TeamNameRepeat { get; set; }
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
        /// UploadMessageSetting
        /// </summary>
        public class UploadMessageSetting
        {
            /// <summary>
            /// Gets or sets AvatarFail
            /// </summary>
            public string AvatarFail { get; set; }

            /// <summary>
            /// Gets or sets FrontCoverFail
            /// </summary>
            public string FrontCoverFail { get; set; }

            /// <summary>
            /// Gets or sets HomePhotoFail
            /// </summary>
            public string HomePhotoFail { get; set; }

            /// <summary>
            /// Gets or sets PhotoFail
            /// </summary>
            public string PhotoFail { get; set; }
        }

        /// <summary>
        /// VerifyCodeMessageSetting
        /// </summary>
        public class VerifyCodeMessageSetting
        {
            /// <summary>
            /// Gets or sets MatchFail
            /// </summary>
            public string MatchFail { get; set; }

            /// <summary>
            /// Gets or sets MatchSuccess
            /// </summary>
            public string MatchSuccess { get; set; }

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