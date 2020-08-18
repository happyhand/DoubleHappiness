using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
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
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamSearchContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.SearchKey)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.SearchKeyEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.SearchKeyEmpty}";
              });
        }
    }
}