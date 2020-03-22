using FluentValidation;

namespace Smtp.Service.Models
{
    /// <summary>
    /// 郵件資料
    /// </summary>
    public class EmailContext
    {
        /// <summary>
        /// Gets or sets Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets Body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets Subject
        /// </summary>
        public string Subject { get; set; }
    }

    /// <summary>
    /// 驗證郵件資料
    /// </summary>
    public class EmailContextValidator : AbstractValidator<EmailContext>
    {
        public EmailContextValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(context => context.Address)
            .NotNull().WithMessage("信箱無效.")
            .NotEmpty().WithMessage("信箱無效.")
            .EmailAddress().WithMessage("信箱格式錯誤.");

            RuleFor(context => context.Subject)
            .NotNull().WithMessage("空白郵件主旨.")
            .NotEmpty().WithMessage("空白郵件主旨.");

            RuleFor(context => context.Body)
            .NotNull().WithMessage("空白郵件內容.")
            .NotEmpty().WithMessage("空白郵件內容.");
        }
    }
}