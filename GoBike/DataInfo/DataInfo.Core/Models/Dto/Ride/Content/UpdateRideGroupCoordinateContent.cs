using DataInfo.Core.Models.Enum;
using FluentValidation;
using Newtonsoft.Json;
using System.Linq;

namespace DataInfo.Core.Models.Dto.Ride.Content
{
    /// <summary>
    /// 更新組隊騎乘座標內容
    /// </summary>
    public class UpdateRideGroupCoordinateContent
    {
        /// <summary>
        /// Gets or sets CoordinateX
        /// </summary>
        public string CoordinateX { get; set; }

        /// <summary>
        /// Gets or sets CoordinateY
        /// </summary>
        public string CoordinateY { get; set; }
    }

    /// <summary>
    /// 驗證更新組隊騎乘座標內容
    /// </summary>
    public class UpdateRideGroupCoordinateContentValidator : AbstractValidator<UpdateRideGroupCoordinateContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public UpdateRideGroupCoordinateContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.CoordinateX)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.CoordinateEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.CoordinateEmpty}";
              });

            RuleFor(content => content.CoordinateY)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.CoordinateEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.CoordinateEmpty}";
              });
        }
    }
}