using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員忘記密碼內容
    /// </summary>
    public class MemberForgetPasswordContent
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

        /// <summary>
        /// Gets or sets VerifierCode
        /// </summary>
        public string VerifierCode { get; set; }
    }

    /// <summary>
    /// 驗證會員忘記密碼內容
    /// </summary>
    public class MemberForgetPasswordContentValidator : AbstractValidator<MemberForgetPasswordContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberForgetPasswordContentValidator()
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

            RuleFor(content => content.VerifierCode)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.VerifyCodeEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.VerifyCodeEmpty}";
              })
              .Length(AppSettingHelper.Appsetting.VerifierCode.Length).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.VerifyCodeFormatError}|{content.VerifierCode}";
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