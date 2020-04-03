using DataInfo.Core.Enums;
using FluentValidation;
using System.Collections.Generic;

namespace DataInfo.Service.Models.Member.Content
{
    /// <summary>
    /// 騎乘資訊內容
    /// </summary>
    public class RideInfoContent
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
    /// 驗證騎乘資訊內容
    /// </summary>
    public class RideInfoContentValidator : AbstractValidator<RideInfoContent>
    {
        public RideInfoContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Time)
            .NotNull().WithMessage("騎乘時間無效.")
            .NotEmpty().WithMessage("騎乘時間無效.")
            .Must(time =>
            {
                return long.TryParse(time, out long value) && value > 0;
            }).WithMessage("騎乘時間無效.");

            RuleFor(content => content.Distance)
            .NotNull().WithMessage("騎乘距離無效.")
            .NotEmpty().WithMessage("騎乘距離無效.")
            .Must(distance =>
            {
                return decimal.TryParse(distance, out decimal value) && value > 0;
            }).WithMessage("騎乘距離無效.");

            RuleFor(content => content.Altitude)
            .NotNull().WithMessage("爬升高度無效.")
            .NotEmpty().WithMessage("爬升高度無效.")
            .Must(altitude =>
            {
                return decimal.TryParse(altitude, out decimal value) && value > 0;
            }).WithMessage("爬升高度無效.");

            RuleFor(content => content.CountyID)
            .Must(countyID =>
            {
                return countyID != (int)CityType.None;
            }).WithMessage("未設定騎乘市區.");

            RuleFor(content => content.Level)
            .Must(level =>
            {
                return level != (int)RideLevelType.None;
            }).WithMessage("未設定騎乘等級.");

            RuleFor(content => content.Route)
            .NotNull().WithMessage("騎乘路徑內容無效.")
            .NotEmpty().WithMessage("無騎乘路徑內容資料.");

            RuleFor(content => content.Photo)
            .NotNull().WithMessage("騎乘封面無效.")
            .NotEmpty().WithMessage("騎乘封面無效.");
        }
    }
}