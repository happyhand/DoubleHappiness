﻿using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 車隊公告內容
    /// </summary>
    public class TeamBulletinContent
    {
        /// <summary>
        /// Gets or sets 公告 ID
        /// </summary>
        public string BulletinID { get; set; }
    }

    /// <summary>
    /// 驗證車隊公告內容
    /// </summary>
    public class TeamBulletinContentValidator : AbstractValidator<TeamBulletinContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamBulletinContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(content => content.BulletinID)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.BulletinIDEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.BulletinIDEmpty}";
              });
        }
    }
}