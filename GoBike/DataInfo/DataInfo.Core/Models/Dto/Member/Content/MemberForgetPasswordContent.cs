using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員忘記密碼內容
    /// </summary>
    public class MemberForgetPasswordContent
    {
        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets VerifierCode
        /// </summary>
        public string VerifierCode { get; set; }
    }

    /// <summary>
    /// 驗證會員忘記密碼內容
    /// </summary>
    public class MemberForgetPasswordContentValidator : AbstractValidator<MemberForgetPasswordContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="isValidateVerifierCode">isValidateVerifierCode</param>
        public MemberForgetPasswordContentValidator(bool isValidateVerifierCode)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Email)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailEmpty)
            .EmailAddress().WithMessage(MessageHelper.Message.ResponseMessage.Member.EmailFormatError);

            if (isValidateVerifierCode)
            {
                RuleFor(content => content.VerifierCode)
                .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.VerifyCode.VerifyCodeEmpty)
                .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.VerifyCode.VerifyCodeEmpty);
            }
        }
    }
}