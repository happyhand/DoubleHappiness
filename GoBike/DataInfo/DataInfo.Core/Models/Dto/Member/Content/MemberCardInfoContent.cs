using DataInfo.Core.Applibs;
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
        public MemberCardInfoContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.MemberID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty);
        }
    }
}