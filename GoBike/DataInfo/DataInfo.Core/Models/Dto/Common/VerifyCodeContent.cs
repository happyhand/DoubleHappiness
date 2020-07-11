using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
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

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }
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
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
             .NotNull().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.EmailEmpty}";
             })
             .NotEmpty().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.EmailEmpty}";
             })
             .EmailAddress().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.EmailFormatError}|Email: {content.Email}";
             });

            RuleFor(content => content.VerifierCode)
             .NotNull().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.VerifyCodeEmpty}";
             })
             .NotEmpty().WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.VerifyCodeEmpty}";
             })
             .Length(AppSettingHelper.Appsetting.VerifierCode.Length).WithMessage(content =>
             {
                 return $"{ResponseErrorMessageType.VerifyCodeFormatError}|{content.VerifierCode}";
             });
        }
    }
}