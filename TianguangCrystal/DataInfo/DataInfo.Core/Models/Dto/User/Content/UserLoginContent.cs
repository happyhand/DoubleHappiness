using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.User.Content
{
    /// <summary>
    /// 使用者登入內容
    /// </summary>
    public class UserLoginContent
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
    /// 驗證使用者登入內容
    /// </summary>
    public class UserLoginContentValidator : AbstractValidator<UserLoginContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public UserLoginContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.User.EmailEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.User.EmailEmpty);

            RuleFor(content => content.Password)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.User.PasswordEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.User.PasswordEmpty);
        }
    }
}