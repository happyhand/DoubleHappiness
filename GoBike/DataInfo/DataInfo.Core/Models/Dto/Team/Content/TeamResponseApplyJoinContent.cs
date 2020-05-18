using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 車隊回覆申請加入內容
    /// </summary>
    public class TeamResponseApplyJoinContent
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

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
    /// 驗證車隊回覆申請加入內容
    /// </summary>
    public class TeamResponseApplyJoinContentValidator : AbstractValidator<TeamResponseApplyJoinContent>
    {
        public TeamResponseApplyJoinContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.MemberID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Member.MemberIDEmpty);

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