using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 取得車隊內容
    /// </summary>
    public class TeamGetContent
    {
        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證取得車隊內容
    /// </summary>
    public class TeamGetContentValidator : AbstractValidator<TeamGetContent>
    {
        public TeamGetContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.TeamID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty);
        }
    }
}