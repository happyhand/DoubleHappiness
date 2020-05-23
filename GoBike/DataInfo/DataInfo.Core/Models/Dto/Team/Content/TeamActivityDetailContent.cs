using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 車隊活動明細內容
    /// </summary>
    public class TeamActivityDetailContent
    {
        /// <summary>
        /// Gets or sets 活動 ID
        /// </summary>
        public string ActID { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證車隊活動明細內容
    /// </summary>
    public class TeamActivityDetailContentValidator : AbstractValidator<TeamActivityDetailContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamActivityDetailContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(content => content.TeamID)
           .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty)
           .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty);

            RuleFor(content => content.ActID)
           .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityIDEmpty)
           .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityIDEmpty);
        }
    }
}