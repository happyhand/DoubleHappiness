using DataInfo.Core.Applibs;
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
        /// <param name="memberID">memberID</param>
        public InteractiveContentValidator(string memberID)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.MemberID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty)
            .NotEqual(memberID).WithMessage(MessageHelper.Message.ResponseMessage.Interactive.TargetError);
        }
    }
}