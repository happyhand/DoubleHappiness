using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員名片內容
    /// </summary>
    public class MemberCardInfoContent
    {
        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }
    }

    /// <summary>
    /// 驗證會員名片內容
    /// </summary>
    public class MemberCardInfoContentValidator : AbstractValidator<MemberCardInfoContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberCardInfoContentValidator()
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