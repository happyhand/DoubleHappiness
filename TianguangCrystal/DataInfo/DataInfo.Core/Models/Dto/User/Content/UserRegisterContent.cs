using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;
using System;

namespace DataInfo.Core.Models.Dto.User.Content
{
    /// <summary>
    /// 使用者註冊內容
    /// </summary>
    public class UserRegisterContent
    {
        /// <summary>
        /// Gets or sets Birthday
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Gender
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Gets or sets Mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets Subscribe
        /// </summary>
        public int Subscribe { get; set; }
    }

    /// <summary>
    /// 驗證使用者註冊內容
    /// </summary>
    public class UserRegisterContentValidator : AbstractValidator<UserRegisterContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public UserRegisterContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.User.EmailEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.User.EmailEmpty)
            .EmailAddress().WithMessage(MessageHelper.Message.ResponseMessage.User.EmailFormatError);

            RuleFor(content => content.Password)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.User.PasswordEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.User.PasswordEmpty)
            .Must(password => { return Utility.ValidatePassword(password); }).WithMessage(MessageHelper.Message.ResponseMessage.User.PasswordFormatError);

            RuleFor(content => content.Name)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.User.NameEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.User.NameEmpty);

            RuleFor(content => content.Gender)
            .Must(Gender =>
            {
                return Gender != (int)GenderType.None;
            }).WithMessage(MessageHelper.Message.ResponseMessage.User.GenderEmpty);

            RuleFor(content => content.Birthday)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.User.BirthdayEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.User.BirthdayEmpty)
            .Must(Birthday =>
            {
                return DateTime.TryParse(Birthday, out DateTime dateTime);
            }).WithMessage(MessageHelper.Message.ResponseMessage.User.BirthdayEmpty);

            RuleFor(content => content.Mobile)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.User.MobileEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.User.MobileEmpty)
            .Must(mobile => { return Utility.ValidateMobile(mobile); }).WithMessage(MessageHelper.Message.ResponseMessage.User.MobileFormatError);
        }
    }
}