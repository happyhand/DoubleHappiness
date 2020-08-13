using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 新增車隊公告內容
    /// </summary>
    public class TeamAddBulletinContent
    {
        /// <summary>
        /// Gets or sets 內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets 天數
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }
    }

    /// <summary>
    /// 驗證新增車隊公告內容
    /// </summary>
    public class TeamAddBulletinContentValidator : AbstractValidator<TeamAddBulletinContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamAddBulletinContentValidator()
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

            RuleFor(content => content.Content)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.BulletinContentEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.BulletinContentEmpty}";
              });

            RuleFor(content => content.Day)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.BulletinDayEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.BulletinDayEmpty}";
              })
              .GreaterThan(default(int)).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.BulletinDayEmpty}|Day: {content.Day}";
              });
        }
    }
}