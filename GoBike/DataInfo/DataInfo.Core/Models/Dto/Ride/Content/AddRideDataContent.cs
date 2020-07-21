using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataInfo.Core.Models.Dto.Ride.Content
{
    /// <summary>
    /// 新增騎乘資訊內容
    /// </summary>
    public class AddRideDataContent
    {
        /// <summary>
        /// Gets or sets Altitude
        /// </summary>
        public string Altitude { get; set; }

        /// <summary>
        /// Gets or sets County
        /// </summary>
        public int County { get; set; }

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
    public class AddRideDataContentValidator : AbstractValidator<AddRideDataContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public AddRideDataContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.Time)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideTimeEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideTimeEmpty}";
              })
              .Must(time =>
              {
                  return long.TryParse(time, out long value) && value > 0;
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideTimeEmpty}|Time: {content.Time}";
              });

            RuleFor(content => content.Distance)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideDistanceEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideDistanceEmpty}";
              })
              .Must(distance =>
              {
                  return decimal.TryParse(distance, out decimal value) && value > 0;
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideDistanceEmpty}|Distance: {content.Distance}";
              });

            RuleFor(content => content.Altitude)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideAltitudeEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideAltitudeEmpty}";
              })
              .Must(altitude =>
              {
                  return decimal.TryParse(altitude, out decimal value) && value > 0;
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideAltitudeEmpty}|Altitude: {content.Altitude}";
              });

            RuleFor(content => content.County)
              .Must(county =>
              {
                  Dictionary<string, string> countyMap = AppSettingHelper.Appsetting.CountyMap;
                  return county >= Convert.ToInt32(countyMap.Keys.FirstOrDefault()) && county <= Convert.ToInt32(countyMap.Keys.LastOrDefault());
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideCountyEmpty}|County: {content.County}";
              });

            RuleFor(content => content.Level)
              .Must(level =>
              {
                  return level >= (int)RideLevelType.Easy && level <= (int)RideLevelType.Hard;
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideLevelEmpty}|Level: {content.Level}";
              });

            RuleFor(content => content.Route)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideRouteEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RideRouteEmpty}";
              });

            RuleFor(content => content.Photo)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RidePhotoEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.RidePhotoEmpty}";
              });
        }
    }
}