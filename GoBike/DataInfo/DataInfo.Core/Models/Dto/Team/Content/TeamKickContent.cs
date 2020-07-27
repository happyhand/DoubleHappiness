using DataInfo.Core.Models.Enum;
using FluentValidation;
using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 踢離車隊隊員內容
    /// </summary>
    public class TeamKickContent
    {
        /// <summary>
        /// Gets or sets 會員 ID 列表
        /// </summary>
        public IEnumerable<string> MemberIDs { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證踢離車隊隊員內容
    /// </summary>
    public class TeamKickContentValidator : AbstractValidator<TeamKickContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamKickContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.MemberIDs)
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
        }
    }
}