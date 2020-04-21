using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Common.Content
{
    public class SendVerifierCodeContent
    {
        /// <summary>
        /// Gets or sets VerifierCode
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// 驗證會員忘記密碼內容
    /// </summary>
    public class SendVerifierCodeContentValidator : AbstractValidator<SendVerifierCodeContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public SendVerifierCodeContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .EmailAddress().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailFormatError);
        }
    }
}