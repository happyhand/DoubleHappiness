using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員請求手機綁定內容
    /// </summary>
    public class MemberRequestMobileBindContent
    {
        /// <summary>
        /// Gets or sets Mobile
        /// </summary>
        public string Mobile { get; set; }
    }

    /// <summary>
    /// 驗證會員請求手機綁定內容
    /// </summary>
    public class MemberRequestMobileBindContentValidator : AbstractValidator<MemberRequestMobileBindContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberRequestMobileBindContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Mobile)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.MobileEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.MobileEmpty)
            .Must(mobile => { return Utility.ValidateMobile(mobile); }).WithMessage(MessageHelper.Message.ResponseMessage.Member.MobileFormatError);
        }
    }
}