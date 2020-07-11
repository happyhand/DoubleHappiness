using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
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
        public MemberMobileBindContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
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