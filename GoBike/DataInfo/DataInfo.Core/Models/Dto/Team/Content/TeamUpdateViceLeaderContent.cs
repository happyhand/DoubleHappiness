using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 更換車隊副隊長內容
    /// </summary>
    public class TeamUpdateViceLeaderContent
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證更換車隊副隊長內容
    /// </summary>
    public class TeamUpdateViceLeaderContentValidator : AbstractValidator<TeamUpdateViceLeaderContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamUpdateViceLeaderContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.MemberID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty);

            RuleFor(content => content.TeamID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty);
        }
    }
}