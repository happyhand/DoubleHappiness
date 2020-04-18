using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員註冊內容
    /// </summary>
    public class MemberRegisterContent
    {
        /// <summary>
        /// Gets or sets ConfirmPassword
        /// </summary>
        public string ConfirmPassword { get; set; }

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
    /// 驗證會員註冊內容
    /// </summary>
    public class MemberRegisterContentValidator : AbstractValidator<MemberRegisterContent>
    {
        public MemberRegisterContentValidator(bool isValidatePassword)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage("信箱無效.")
            .NotEmpty().WithMessage("信箱無效.")
            .EmailAddress().WithMessage("信箱格式錯誤.");

            if (isValidatePassword)
            {
                RuleFor(content => content.Password)
                  .NotNull().WithMessage("密碼無效.")
                  .NotEmpty().WithMessage("密碼無效.")
                  .Must(password => { return Utility.ValidatePassword(password); }).WithMessage("密碼格式錯誤.");
                RuleFor(content => content.ConfirmPassword)
                  .NotNull().WithMessage("未輸入相同密碼.")
                  .NotEmpty().WithMessage("未輸入相同密碼.")
                  .Equal(content => content.Password).WithMessage("未輸入相同密碼.");
            }
        }
    }
}