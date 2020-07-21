using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Ride.Content
{
    /// <summary>
    /// 回覆組隊騎乘內容
    /// </summary>
    public class ReplyRideGroupContent
    {
        /// <summary>
        /// Gets or sets 回覆類別
        /// </summary>
        public int Reply { get; set; }
    }

    /// <summary>
    /// 驗證回覆組隊騎乘內容
    /// </summary>
    public class ReplyRideGroupContentValidator : AbstractValidator<ReplyRideGroupContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public ReplyRideGroupContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Reply)
              .Must(reply =>
              {
                  return reply == (int)RideReplytGroupType.Allow || reply == (int)RideReplytGroupType.Reject;
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.ReplyFail}|Reply: {content.Reply}";
              });
        }
    }
}