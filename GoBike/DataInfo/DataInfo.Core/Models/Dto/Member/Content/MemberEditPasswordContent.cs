using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員修改密碼內容
    /// </summary>
    public class MemberEditPasswordContent
    {
        /// <summary>
        /// Gets or sets 確認密碼
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets 目前密碼
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Gets or sets 新密碼
        /// </summary>
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// 驗證會員修改密碼內容
    /// </summary>
    public class MemberEditPasswordContentValidator : AbstractValidator<MemberEditPasswordContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberEditPasswordContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.CurrentPassword)
            .NotNull().WithMessage("目前密碼無效.")
            .NotEmpty().WithMessage("目前密碼無效.");
            RuleFor(content => content.NewPassword)
            .NotNull().WithMessage("新密碼無效.")
            .NotEmpty().WithMessage("新密碼無效.");
            RuleFor(content => content.ConfirmPassword)
            .NotNull().WithMessage("未輸入相同新密碼.")
            .NotEmpty().WithMessage("未輸入相同新密碼.")
            .Equal(content => content.NewPassword).WithMessage("未輸入相同新密碼.");
        }
    }
}