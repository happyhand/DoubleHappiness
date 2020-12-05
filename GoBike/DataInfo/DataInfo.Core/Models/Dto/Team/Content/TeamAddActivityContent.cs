using DataInfo.Core.Models.Dto.Team.Content.data;
using DataInfo.Core.Models.Enum;
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
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.ActDate)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamActivityActDateEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamActivityActDateEmpty}";
              })
              .Must(actDate =>
              {
                  return DateTime.TryParse(actDate, out DateTime dateTime);
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamActivityActDateFail}|ActDate: {content.ActDate}";
              });

            RuleFor(content => content.MeetTime)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamActivityMeetTimeEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamActivityMeetTimeEmpty}";
              })
              .Must(meetTime =>
              {
                  return DateTime.TryParse(meetTime, out DateTime dateTime);
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamActivityMeetTimeFail}|MeetTime: {content.MeetTime}";
              });

            //RuleFor(content => content.MaxAltitude)
            //  .NotNull().WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.TeamActivityMaxAltitudeEmpty}";
            //  })
            //  .NotEmpty().WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.TeamActivityMaxAltitudeEmpty}";
            //  })
            //  .GreaterThan(default(float)).WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.TeamActivityMaxAltitudeFail}|MaxAltitude: {content.MaxAltitude}";
            //  });

            //RuleFor(content => content.TotalDistance)
            //  .NotNull().WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.TeamActivityTotalDistanceEmpty}";
            //  })
            //  .NotEmpty().WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.TeamActivityTotalDistanceEmpty}";
            //  })
            //  .GreaterThan(default(float)).WithMessage(content =>
            //  {
            //      return $"{ResponseErrorMessageType.TeamActivityTotalDistanceFail}|TotalDistance: {content.TotalDistance}";
            //  });

            RuleFor(content => content.Title)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamActivityTitleEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.TeamActivityTitleEmpty}";
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