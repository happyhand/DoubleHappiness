using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 回覆邀請加入車隊內容
    /// </summary>
    public class TeamResponseInviteJoinContent
    {
        /// <summary>
        /// Gets or sets 回覆類別
        /// </summary>
        public int ResponseType { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證回覆邀請加入車隊內容
    /// </summary>
    public class TeamResponseInviteJoinContentValidator : AbstractValidator<TeamResponseInviteJoinContent>
    {
        public TeamResponseInviteJoinContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(content => content.TeamID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty);

            RuleFor(content => content.ResponseType)
            .Must(ApplyType =>
            {
                return ApplyType != (int)TeamResponseType.None;
            }).WithMessage(MessageHelper.Message.ResponseMessage.Team.ResponseStatusEmpty);
        }
    }
}