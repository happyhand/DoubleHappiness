using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Ride.Content
{
    /// <summary>
    /// 組隊騎乘通知內容
    /// </summary>
    public class RideGroupNotifyContent
    {
        /// <summary>
        /// Gets or sets 回覆類別
        /// </summary>
        public int Action { get; set; }
    }

    /// <summary>
    /// 驗證組隊騎乘通知內容
    /// </summary>
    public class RideGroupNotifyContentValidator : AbstractValidator<RideGroupNotifyContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public RideGroupNotifyContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Action)
              .Must(action =>
              {
                  return action != (int)ActionType.None;
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.NotifyFail}|Action: {content.Action}";
              });
        }
    }
}