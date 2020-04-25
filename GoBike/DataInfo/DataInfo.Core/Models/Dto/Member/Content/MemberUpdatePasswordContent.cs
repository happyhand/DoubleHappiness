using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員更新密碼內容
    /// </summary>
    public class MemberUpdatePasswordContent
    {
        /// <summary>
        /// Gets or sets 確認密碼
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets 新密碼
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets 密碼
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// 驗證會員更新密碼內容
    /// </summary>
    public class MemberUpdatePasswordContentValidator : AbstractValidator<MemberUpdatePasswordContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberUpdatePasswordContentValidator(bool isIgnoreOldPassword)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            if (!isIgnoreOldPassword)
            {
                RuleFor(content => content.Password)
                .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordEmpty)
                .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordEmpty);
            }

            RuleFor(content => content.NewPassword)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordEmpty);
            RuleFor(content => content.ConfirmPassword)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordNotMatch)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordNotMatch)
            .Equal(content => content.NewPassword).WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordNotMatch);
        }
    }
}