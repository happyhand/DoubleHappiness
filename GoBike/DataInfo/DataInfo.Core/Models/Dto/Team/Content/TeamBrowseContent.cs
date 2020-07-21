using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 瀏覽車隊內容
    /// </summary>
    public class TeamBrowseContent
    {
        /// <summary>
        /// Gets or sets 所在地
        /// </summary>
        public int County { get; set; }
    }

    /// <summary>
    /// 驗證瀏覽車隊內容
    /// </summary>
    public class TeamBrowseContentValidator : AbstractValidator<TeamBrowseContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamBrowseContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.County)
              .Must(county =>
              {
                  Dictionary<string, string> countyMap = AppSettingHelper.Appsetting.CountyMap;
                  return county >= Convert.ToInt32(countyMap.Keys.FirstOrDefault()) && county <= Convert.ToInt32(countyMap.Keys.LastOrDefault());
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.CountyEmpty}|County: {content.County}";
              });
        }
    }
}