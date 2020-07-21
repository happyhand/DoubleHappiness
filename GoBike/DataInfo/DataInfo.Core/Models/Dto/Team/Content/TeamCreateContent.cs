using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Gets or sets 所在地
        /// </summary>
        public int County { get; set; }

        /// <summary>
        /// Gets or sets 審查狀態
        /// </summary>
        public int ExamineStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊封面圖片路徑
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 搜尋狀態
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
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamCreateContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            //RuleFor(content => content.Avatar)
            //.NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.AvatarEmpty)
            //.NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.AvatarEmpty);

            //RuleFor(content => content.FrontCover)
            //.NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.FrontCoverEmpty)
            //.NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.FrontCoverEmpty);

            RuleFor(content => content.County)
            .Must(county =>
            {
                Dictionary<string, string> countyMap = AppSettingHelper.Appsetting.CountyMap;
                return county >= Convert.ToInt32(countyMap.Keys.FirstOrDefault()) && county <= Convert.ToInt32(countyMap.Keys.LastOrDefault());
            }).WithMessage(MessageHelper.Message.ResponseMessage.Team.CountyEmpty);

            RuleFor(content => content.ExamineStatus)
           .Must(examineStatus =>
           {
               return examineStatus == (int)TeamExamineStatusType.Close || examineStatus == (int)TeamExamineStatusType.Open;
           }).WithMessage(MessageHelper.Message.ResponseMessage.Team.ExamineStatusEmpty);

            RuleFor(content => content.SearchStatus)
           .Must(searchStatus =>
           {
               return searchStatus == (int)TeamSearchStatusType.Close || searchStatus == (int)TeamSearchStatusType.Open;
           }).WithMessage(MessageHelper.Message.ResponseMessage.Team.SearchStatusEmpty);

            //RuleFor(content => content.TeamInfo)
            //.NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamInfoEmpty)
            //.NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamInfoEmpty);

            RuleFor(content => content.TeamName)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamNameEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamNameEmpty);
        }
    }
}