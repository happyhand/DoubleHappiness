using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員更新推播 Token 內容
    /// </summary>
    public class MemberUpdateNotifyTokenContent
    {
        /// <summary>
        /// Gets or sets 裝置 Token
        /// </summary>
        public string NotifyToken { get; set; }
    }

    /// <summary>
    /// 驗證會員更新推播 Token 內容
    /// </summary>
    public class MemberUpdateNotifyTokenContentValidator : AbstractValidator<MemberUpdateNotifyTokenContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberUpdateNotifyTokenContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.NotifyToken)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.UpdateFail}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.UpdateFail}";
              });
        }
    }
}