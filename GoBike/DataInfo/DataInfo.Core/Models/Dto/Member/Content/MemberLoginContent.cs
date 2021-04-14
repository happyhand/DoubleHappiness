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
        /// Gets or sets Token
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
        }
    }
}