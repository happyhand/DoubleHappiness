using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Interactive.Content
{
    /// <summary>
    /// 互動內容
    /// </summary>
    public class InteractiveContent
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }
    }

    /// <summary>
    /// 驗證互動內容
    /// </summary>
    public class InteractiveContentValidator : AbstractValidator<InteractiveContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public InteractiveContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.MemberID)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.MemberIDEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.MemberIDEmpty}";
              });
        }
    }
}