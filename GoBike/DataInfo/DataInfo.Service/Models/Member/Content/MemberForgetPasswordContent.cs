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
    }

    /// <summary>
    /// 驗證會員忘記密碼內容
    /// </summary>
    public class MemberForgetPasswordContentValidator : AbstractValidator<MemberForgetPasswordContent>
    {
        public MemberForgetPasswordContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage("信箱無效.")
            .NotEmpty().WithMessage("信箱無效.")
            .EmailAddress().WithMessage("信箱格式錯誤.");
        }
    }
}