using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;
using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Ride.Content
{
    /// <summary>
    /// 新增騎乘資訊內容
    /// </summary>
    public class AddRideInfoContent
    {
        /// <summary>
        /// Gets or sets Altitude
        /// </summary>
        public string Altitude { get; set; }

        /// <summary>
        /// Gets or sets CountyID
        /// </summary>
        public int CountyID { get; set; }

        /// <summary>
        /// Gets or sets Distance
        /// </summary>
        public string Distance { get; set; }

        /// <summary>
        /// Gets or sets Level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets Photo
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets Route
        /// </summary>
        public IEnumerable<IEnumerable<string>> Route { get; set; }

        /// <summary>
        /// Gets or sets ShareContent
        /// </summary>
        public IEnumerable<IEnumerable<string>> ShareContent { get; set; }

        /// <summary>
        /// Gets or sets SharedType
        /// </summary>
        public int SharedType { get; set; }

        /// <summary>
        /// Gets or sets Time
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// 驗證新增騎乘資訊內容
    /// </summary>
    public class AddRideInfoContentValidator : AbstractValidator<AddRideInfoContent>
    {
        public AddRideInfoContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Time)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Ride.TimeEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Ride.TimeEmpty)
            .Must(time =>
            {
                return long.TryParse(time, out long value) && value > 0;
            }).WithMessage(MessageHelper.Message.ResponseMessage.Ride.TimeEmpty);

            RuleFor(content => content.Distance)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Ride.DistanceEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Ride.DistanceEmpty)
            .Must(distance =>
            {
                return decimal.TryParse(distance, out decimal value) && value > 0;
            }).WithMessage(MessageHelper.Message.ResponseMessage.Ride.DistanceEmpty);

            RuleFor(content => content.Altitude)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Ride.AltitudeEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Ride.AltitudeEmpty)
            .Must(altitude =>
            {
                return decimal.TryParse(altitude, out decimal value) && value > 0;
            }).WithMessage(MessageHelper.Message.ResponseMessage.Ride.AltitudeEmpty);

            RuleFor(content => content.CountyID)
            .Must(countyID =>
            {
                return countyID != (int)CountyType.None;
            }).WithMessage(MessageHelper.Message.ResponseMessage.Ride.CountyIDEmpty);

            RuleFor(content => content.Level)
            .Must(level =>
            {
                return level != (int)RideLevelType.None;
            }).WithMessage(MessageHelper.Message.ResponseMessage.Ride.LevelEmpty);

            RuleFor(content => content.Route)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Ride.RouteEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Ride.RouteEmpty);

            RuleFor(content => content.Photo)
            .NotNull().WithMessage(MessageHelper.Message.ResponseMessage.Ride.PhotoEmpty)
            .NotEmpty().WithMessage(MessageHelper.Message.ResponseMessage.Ride.PhotoEmpty);
        }
    }
}