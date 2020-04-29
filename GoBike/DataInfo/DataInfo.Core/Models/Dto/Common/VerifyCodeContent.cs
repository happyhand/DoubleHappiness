using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Common
{
    /// <summary>
    /// 驗證碼內容
    /// </summary>
    public class VerifyCodeContent
    {
        /// <summary>
        /// Gets or sets VerifierCode
        /// </summary>
        public string VerifierCode { get; set; }
    }

    /// <summary>
    /// 驗證驗證碼內容
    /// </summary>
    public class VerifyCodeContentValidator : AbstractValidator<VerifyCodeContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public VerifyCodeContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.VerifierCode)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.VerifyCode.VerifyCodeEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.VerifyCode.VerifyCodeEmpty);
        }
    }
}