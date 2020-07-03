using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員註冊內容
    /// </summary>
    public class MemberRegisterContent
    {
        /// <summary>
        /// Gets or sets ConfirmPassword
        /// </summary>
        public string ConfirmPassword { get; set; }

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
    /// 驗證會員註冊內容
    /// </summary>
    public class MemberRegisterContentValidator : AbstractValidator<MemberRegisterContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberRegisterContentValidator()
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
              })
              .EmailAddress().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.EmailFormatError}|Email: {content.Email}";
              });
            RuleFor(content => content.Password)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.PasswordEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.PasswordEmpty}";
              })
              .Must(password => { return Utility.ValidatePassword(password); }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.PasswordFormatError}|Password: {content.Password}";
              });
            RuleFor(content => content.ConfirmPassword)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.ConfirmPasswordEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.ConfirmPasswordEmpty}";
              })
              .Equal(content => content.Password).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.ConfirmPasswordNotMatch}|Password: {content.Password} ConfirmPassword: {content.ConfirmPassword}";
              });
        }
    }
}