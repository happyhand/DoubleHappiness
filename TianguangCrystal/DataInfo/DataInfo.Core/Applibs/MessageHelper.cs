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
            /// Gets or sets MobileExist
            /// </summary>
            public string MobileExist { get; set; }

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
            /// Gets or sets Login
            /// </summary>
            public LoginMessageSetting Login { get; set; }

            /// <summary>
            /// Gets or sets Register
            /// </summary>
            public RegisterMessageSetting Register { get; set; }

            /// <summary>
            /// Gets or sets Update
            /// </summary>
            public UpdateMessageSetting Update { get; set; }

            /// <summary>
            /// Gets or sets User
            /// </summary>
            public UserMessageSetting User { get; set; }
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
        /// UserMessageSetting
        /// </summary>
        public class UserMessageSetting
        {
            /// <summary>
            /// Gets or sets BirthdayEmpty
            /// </summary>
            public string BirthdayEmpty { get; set; }

            /// <summary>
            /// Gets or sets EmailEmpty
            /// </summary>
            public string EmailEmpty { get; set; }

            /// <summary>
            /// Gets or sets EmailEmpty
            /// </summary>
            public string EmailFormatError { get; set; }

            /// <summary>
            /// Gets or sets GenderEmpty
            /// </summary>
            public string GenderEmpty { get; set; }

            /// <summary>
            /// Gets or sets MobileEmpty
            /// </summary>
            public string MobileEmpty { get; set; }

            /// <summary>
            /// Gets or sets MobileFormatError
            /// </summary>
            public string MobileFormatError { get; set; }

            /// <summary>
            /// Gets or sets NameEmpty
            /// </summary>
            public string NameEmpty { get; set; }

            /// <summary>
            /// Gets or sets PasswordEmpty
            /// </summary>
            public string PasswordEmpty { get; set; }

            /// <summary>
            /// Gets or sets PasswordFormatError
            /// </summary>
            public string PasswordFormatError { get; set; }
        }

        #endregion ResponseMessage
    }
}