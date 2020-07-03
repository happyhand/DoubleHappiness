using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員更新密碼內容
    /// </summary>
    public class MemberUpdatePasswordContent
    {
        /// <summary>
        /// Gets or sets 確認密碼
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets 新密碼
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets 密碼
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// 驗證會員更新密碼內容
    /// </summary>
    public class MemberUpdatePasswordContentValidator : AbstractValidator<MemberUpdatePasswordContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberUpdatePasswordContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
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

            RuleFor(content => content.NewPassword)
             .NotNull().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.NewPasswordEmpty}";
             })
             .NotEmpty().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.NewPasswordEmpty}";
             })
             .Must(password => { return Utility.ValidatePassword(password); }).WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.NewPasswordFormatError}|NewPassword: {content.NewPassword}";
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
              .Equal(content => content.NewPassword).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.ConfirmPasswordNotMatch}|NewPassword: {content.NewPassword} ConfirmPassword: {content.ConfirmPassword}";
              });
        }
    }
}