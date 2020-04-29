﻿using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 建立車隊內容
    /// </summary>
    public class TeamCreateContent
    {
        /// <summary>
        /// Gets or sets 車隊頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets CountyID
        /// </summary>
        public int CountyID { get; set; }

        /// <summary>
        /// Gets or sets ExamineStatus
        /// </summary>
        public int ExamineStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊封面路徑
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets SearchStatus
        /// </summary>
        public int SearchStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊簡介
        /// </summary>
        public string TeamInfo { get; set; }

        /// <summary>
        /// Gets or sets 車隊名稱
        /// </summary>
        public string TeamName { get; set; }
    }

    /// <summary>
    /// 驗證會員註冊內容
    /// </summary>
    public class TeamCreateContentValidator : AbstractValidator<TeamCreateContent>
    {
        public TeamCreateContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Avatar)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.AvatarEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.AvatarEmpty);

            RuleFor(content => content.FrontCover)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.FrontCoverEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.FrontCoverEmpty);

            RuleFor(content => content.CountyID)
            .Must(countyID =>
            {
                return countyID != (int)CountyType.None;
            }).WithMessage(MessageHelper.Message.ResponseMessage.Team.CountyIDEmpty);

            RuleFor(content => content.ExamineStatus)
           .Must(examineStatus =>
           {
               return examineStatus != (int)TeamExamineStatusType.None;
           }).WithMessage(MessageHelper.Message.ResponseMessage.Team.ExamineStatusEmpty);

            RuleFor(content => content.SearchStatus)
           .Must(searchStatus =>
           {
               return searchStatus != (int)TeamSearchStatusType.None;
           }).WithMessage(MessageHelper.Message.ResponseMessage.Team.SearchStatusEmpty);

            RuleFor(content => content.TeamInfo)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamInfoEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamInfoEmpty);

            RuleFor(content => content.TeamName)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamNameEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamNameEmpty);
        }
    }
}