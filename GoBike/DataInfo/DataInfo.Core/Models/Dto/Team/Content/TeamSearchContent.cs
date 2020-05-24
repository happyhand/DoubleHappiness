using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 搜尋車隊內容
    /// </summary>
    public class TeamSearchContent
    {
        /// <summary>
        /// Gets or sets 關鍵字
        /// </summary>
        public string SearchKey { get; set; }
    }

    /// <summary>
    /// 驗證搜尋車隊內容
    /// </summary>
    public class TeamSearchContentValidator : AbstractValidator<TeamSearchContent>
    {
        public TeamSearchContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.SearchKey)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.SearchKeyEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.SearchKeyEmpty);
        }
    }
}