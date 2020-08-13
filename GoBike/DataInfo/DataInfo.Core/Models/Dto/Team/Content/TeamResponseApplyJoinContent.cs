using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 回覆申請加入車隊內容
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
    /// 驗證回覆申請加入車隊內容
    /// </summary>
    public class TeamResponseApplyJoinContentValidator : AbstractValidator<TeamResponseApplyJoinContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamResponseApplyJoinContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.MemberID)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.MemberIDEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.MemberIDEmpty}";
              });

            RuleFor(content => content.TeamID)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamIDEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamIDEmpty}";
              });

            RuleFor(content => content.ResponseType)
            .Must(ApplyType =>
            {
                return ApplyType == (int)TeamResponseType.Reject || ApplyType == (int)TeamResponseType.Allow;
            }).WithMessage(content =>
            {
                return $"{ResponseErrorMessageType.ReplyFail}|ResponseType: {content.ResponseType}";
            });
        }
    }
}