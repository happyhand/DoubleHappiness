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
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .EmailAddress().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailFormatError);

            if (isValidatePassword)
            {
                RuleFor(content => content.Password)
                  .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordEmpty)
                  .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordEmpty)
                  .Must(password => { return Utility.ValidatePassword(password); }).WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordFormatError);
                RuleFor(content => content.ConfirmPassword)
                  .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordNotMatch)
                  .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordNotMatch)
                  .Equal(content => content.Password).WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordNotMatch);
            }
        }
    }
}