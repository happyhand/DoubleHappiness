using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員登入內容
    /// </summary>
    public class MemberLoginContent
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets 登入來源
        /// </summary>
        public int LoginSource { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets FireBase認證用的Token
        /// </summary>
        public string Token { get; set; }
    }

    /// <summary>
    /// 驗證會員登入內容
    /// </summary>
    public class MemberLoginContentValidator : AbstractValidator<MemberLoginContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberLoginContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Token)
                .NotNull().WithMessage(content =>
                {
                    return $"{ResponseErrorMessageType.TokenEmpty}";
                })
                .NotEmpty().WithMessage(content =>
                {
                    return $"{ResponseErrorMessageType.TokenEmpty}";
                });
            RuleFor(content => content.LoginSource)
              .Must(loginSource =>
              {
                  return loginSource >= (int)LoginSourceType.Normal && loginSource <= (int)LoginSourceType.Google;
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.LoginSourceFail}|LoginSource: {content.LoginSource}";
              });
            //RuleFor(content => content.Email)
            //  .NotNull().WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.EmailEmpty}";
            //  })
            //  .NotEmpty().WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.EmailEmpty}";
            //  });
            //.EmailAddress().WithMessage(content =>
            //{
            //    return $"{ResponseErrorMessageType.EmailFormatError}|Email: {content.Email}";
            //});
            //RuleFor(content => content.Password)
            //  .NotNull().WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.PasswordEmpty}";
            //  })
            //  .NotEmpty().WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.PasswordEmpty}";
            //  });
            //.Must(password => { return Utility.ValidatePassword(password); }).WithMessage(content =>
            //{
            //    return $"{ResponseErrorMessageType.PasswordFormatError}|Password: {content.Password}";
            //});
        }
    }
}