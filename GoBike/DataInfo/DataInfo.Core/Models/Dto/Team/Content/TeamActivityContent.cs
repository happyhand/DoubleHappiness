using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 車隊活動內容
    /// </summary>
    public class TeamActivityContent
    {
        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證車隊活動內容
    /// </summary>
    public class TeamActivityContentValidator : AbstractValidator<TeamActivityContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamActivityContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(content => content.TeamID)
           .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty)
           .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty);
        }
    }
}