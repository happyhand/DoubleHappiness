using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 互動內容
    /// </summary>
    public class InteractiveContent
    {
        /// <summary>
        /// Gets or sets 目標會員ID
        /// </summary>
        public string TargetID { get; set; }
    }

    /// <summary>
    /// 驗證互動內容
    /// </summary>
    public class InteractiveContentValidator : AbstractValidator<InteractiveContent>
    {
        public InteractiveContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.TargetID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty);
        }
    }
}