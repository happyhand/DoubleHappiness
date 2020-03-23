using FluentValidation;

namespace DataInfo.Service.Models.Member.Content
{
    /// <summary>
    /// 會員忘記密碼內容
    /// </summary>
    public class MemberForgetPasswordContent
    {
        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets VerifierCode
        /// </summary>
        public string VerifierCode { get; set; }
    }

    /// <summary>
    /// 驗證會員忘記密碼內容
    /// </summary>
    public class MemberForgetPasswordContentValidator : AbstractValidator<MemberForgetPasswordContent>
    {
        public MemberForgetPasswordContentValidator(bool isValidateVerifierCode)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage("信箱無效.")
            .NotEmpty().WithMessage("信箱無效.")
            .EmailAddress().WithMessage("信箱格式錯誤.");

            if (isValidateVerifierCode)
            {
                RuleFor(content => content.VerifierCode)
                .NotNull().WithMessage("驗證碼無效.")
                .NotEmpty().WithMessage("驗證碼無效.");
            }
        }
    }
}