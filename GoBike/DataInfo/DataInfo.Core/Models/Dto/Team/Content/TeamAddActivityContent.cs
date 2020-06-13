using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Dto.Team.Content.data;
using FluentValidation;
using System;
using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 新增車隊活動內容
    /// </summary>
    public class TeamAddActivityContent
    {
        /// <summary>
        /// Gets or sets 活動日期
        /// </summary>
        public string ActDate { get; set; }

        /// <summary>
        /// Gets or sets 最高海拔
        /// </summary>
        public float MaxAltitude { get; set; }

        /// <summary>
        /// Gets or sets 集合時間
        /// </summary>
        public string MeetTime { get; set; }

        /// <summary>
        /// Gets or sets 活動圖片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets 路線
        /// </summary>
        public IEnumerable<Route> Routes { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }

        /// <summary>
        /// Gets or sets 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets 總距離
        /// </summary>
        public float TotalDistance { get; set; }
    }

    /// <summary>
    /// 驗證新增車隊活動內容
    /// </summary>
    public class TeamAddActivityContentValidator : AbstractValidator<TeamAddActivityContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamAddActivityContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.ActDate)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityDateEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityDateEmpty)
            .Must(actDate =>
            {
                return DateTime.TryParse(actDate, out DateTime dateTime);
            }).WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityDateEmpty);

            RuleFor(content => content.MeetTime)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityMeetTimeEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityMeetTimeEmpty)
            .Must(meetTime =>
            {
                return DateTime.TryParse(meetTime, out DateTime dateTime);
            }).WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityMeetTimeEmpty);

            RuleFor(content => content.MaxAltitude)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityAltitudeEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityAltitudeEmpty)
            .GreaterThan(default(float)).WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityAltitudeEmpty);

            RuleFor(content => content.TotalDistance)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityDistanceEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityDistanceEmpty)
            .GreaterThan(default(float)).WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityDistanceEmpty);

            RuleFor(content => content.Title)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityTitleEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityTitleEmpty);

            RuleFor(content => content.TeamID)
           .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty)
           .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty);

            RuleFor(content => content.Photo)
           .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityPhotoEmpty)
           .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.ActivityPhotoEmpty);
        }
    }
}