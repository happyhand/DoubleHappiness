using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 搜尋會員內容
    /// </summary>
    public class MemberSearchContent
    {
        /// <summary>
        /// Gets or sets SearchKey
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// Gets or sets UseFuzzySearch
        /// </summary>
        public int UseFuzzySearch { get; set; }
    }

    /// <summary>
    /// 驗證搜尋會員內容
    /// </summary>
    public class MemberSearchContentValidator : AbstractValidator<MemberSearchContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberSearchContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.SearchKey)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.SearchEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.SearchEmpty);
        }
    }
}