using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員手機綁定內容
    /// </summary>
    public class MemberMobileBindContent
    {
        /// <summary>
        /// Gets or sets Mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets VerifierCode
        /// </summary>
        public string VerifierCode { get; set; }
    }

    /// <summary>
    /// 驗證會員手機綁定內容
    /// </summary>
    public class MemberMobileBindContentValidator : AbstractValidator<MemberMobileBindContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="isValidateVerifierCode">isValidateVerifierCode</param>
        public MemberMobileBindContentValidator(bool isValidateVerifierCode)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Mobile)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.MobileEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.MobileEmpty)
            .Must(mobile => { return Utility.ValidateMobile(mobile); }).WithMessage(MessageHelper.Message.ResponseMessage.Member.MobileFormatError);

            if (isValidateVerifierCode)
            {
                RuleFor(content => content.VerifierCode)
                .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.VerifyCode.VerifyCodeEmpty)
                .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.VerifyCode.VerifyCodeEmpty);
            }
        }
    }
}