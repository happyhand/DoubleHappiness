using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Service.Models.Member.Content
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
            .NotNull().WithMessage("手機號碼無效.")
            .NotEmpty().WithMessage("手機號碼無效.")
            .Must(mobile => { return Utility.ValidateMobile(mobile); }).WithMessage("手機號碼格式錯誤.");

            if (isValidateVerifierCode)
            {
                RuleFor(content => content.VerifierCode)
                .NotNull().WithMessage("驗證碼無效.")
                .NotEmpty().WithMessage("驗證碼無效.");
            }
        }
    }
}