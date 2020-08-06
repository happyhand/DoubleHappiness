﻿using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 更換車隊隊長內容
    /// </summary>
    public class TeamChangeLeaderContent
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
    /// 驗證更換車隊隊長內容
    /// </summary>
    public class TeamChangeLeaderContentValidator : AbstractValidator<TeamChangeLeaderContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamChangeLeaderContentValidator()
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
        }
    }
}