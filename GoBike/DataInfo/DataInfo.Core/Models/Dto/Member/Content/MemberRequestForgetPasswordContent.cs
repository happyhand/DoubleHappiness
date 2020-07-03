using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員請求忘記密碼內容
    /// </summary>
    public class MemberRequestForgetPasswordContent
    {
        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// 驗證會員請求忘記密碼內容
    /// </summary>
    public class MemberRequestForgetPasswordContentValidator : AbstractValidator<MemberRequestForgetPasswordContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberRequestForgetPasswordContentValidator()
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
        }
    }
}