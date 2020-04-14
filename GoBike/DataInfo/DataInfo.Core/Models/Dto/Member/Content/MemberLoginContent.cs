using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員登入內容
    /// </summary>
    public class MemberLoginContent
    {
        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// 驗證會員登入內容
    /// </summary>
    public class MemberLoginContentValidator : AbstractValidator<MemberLoginContent>
    {
        public MemberLoginContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage("信箱無效.")
            .NotEmpty().WithMessage("信箱無效.")
            .EmailAddress().WithMessage("信箱格式錯誤.");
            RuleFor(content => content.Password)
            .NotNull().WithMessage("密碼無效.")
            .NotEmpty().WithMessage("密碼無效.");
        }
    }
}