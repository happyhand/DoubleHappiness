using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 申請加入車隊內容
    /// </summary>
    public class TeamApplyJoinContent
    {
        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證申請加入車隊內容內容
    /// </summary>
    public class TeamApplyJoinContentValidator : AbstractValidator<TeamApplyJoinContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamApplyJoinContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.TeamID)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamIDEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamIDEmpty}";
              });
        }
    }
}