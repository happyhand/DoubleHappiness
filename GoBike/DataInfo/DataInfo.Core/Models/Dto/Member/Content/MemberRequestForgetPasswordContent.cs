using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員請求忘記密碼內容
    /// </summary>
    public class MemberRequestForgetPasswordContent
    {
        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// 驗證會員請求忘記密碼內容
    /// </summary>
    public class MemberRequestForgetPasswordContentValidator : AbstractValidator<MemberRequestForgetPasswordContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberRequestForgetPasswordContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .EmailAddress().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailFormatError);
        }
    }
}