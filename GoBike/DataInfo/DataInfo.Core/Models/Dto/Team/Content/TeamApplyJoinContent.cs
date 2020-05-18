using DataInfo.Core.Applibs;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 車隊申請加入內容
    /// </summary>
    public class TeamApplyJoinContent
    {
        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證會員註冊內容
    /// </summary>
    public class TeamApplyJoinContentValidator : AbstractValidator<TeamApplyJoinContent>
    {
        public TeamApplyJoinContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.TeamID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty);
        }
    }
}