using DataInfo.Core.Applibs;
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
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .EmailAddress().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailFormatError);
            RuleFor(content => content.Password)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordEmpty)
            .Must(password => { return Utility.ValidatePassword(password); }).WithMessage(MessageHelper.Message.ResponseMessage.Member.PasswordFormatError);
        }
    }
}