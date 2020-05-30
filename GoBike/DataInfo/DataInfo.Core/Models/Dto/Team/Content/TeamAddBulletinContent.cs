using DataInfo.Core.Applibs;
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
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(content => content.TeamID)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.TeamIDEmpty);

            RuleFor(content => content.Content)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.BulletinIDEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.BulletinIDEmpty);

            RuleFor(content => content.Day)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Team.BulletinDayEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Team.BulletinDayEmpty)
            .GreaterThan(default(int)).WithMessage(MessageHelper.Message.ResponseMessage.Team.BulletinDayEmpty);
        }
    }
}