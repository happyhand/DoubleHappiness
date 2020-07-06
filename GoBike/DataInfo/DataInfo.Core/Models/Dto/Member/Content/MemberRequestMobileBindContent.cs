using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
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
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Mobile)
             .NotNull().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.MobileEmpty}";
             })
             .NotEmpty().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.MobileEmpty}";
             })
             .Must(mobile => { return Utility.ValidateMobile(mobile); }).WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.MobileFormatError}|Mobile: {content.Mobile}";
             });
        }
    }
}