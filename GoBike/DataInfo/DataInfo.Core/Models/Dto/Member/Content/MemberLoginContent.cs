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
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }
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
            RuleFor(content => content.Email)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.EmailEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.EmailEmpty}";
              });
            //.EmailAddress().WithMessage(content =>
            //{
            //    return $"{ResponseErrorMessageType.EmailFormatError}|Email: {content.Email}";
            //});
            RuleFor(content => content.Password)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.PasswordEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.PasswordEmpty}";
              });
            //.Must(password => { return Utility.ValidatePassword(password); }).WithMessage(content =>
            //{
            //    return $"{ResponseErrorMessageType.PasswordFormatError}|Password: {content.Password}";
            //});
        }
    }
}