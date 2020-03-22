using FluentValidation;

namespace DataInfo.Service.Models.Member.Content
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
        public MemberSearchContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.SearchKey)
            .NotNull().WithMessage("未提供搜尋內容.")
            .NotEmpty().WithMessage("未提供搜尋內容.");
        }
    }
}