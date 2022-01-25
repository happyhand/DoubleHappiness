using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataInfo.Core.Models.Dto.Ride.Content
{
    /// <summary>
    /// 新增騎乘路線資訊內容
    /// </summary>
    public class AddRideRouteDataContent
    {
        /// <summary>
        /// Gets or sets 騎乘 ID
        /// </summary>
        public string RideID { get; set; }

        /// <summary>
        /// Gets or sets 路線資訊
        /// </summary>
        public IEnumerable<IEnumerable<string>> Route { get; set; }

        /// <summary>
        /// Gets or sets 索引總數
        /// </summary>
        public int IndexCount { get; set; }

        /// <summary>
        /// Gets or sets 該筆紀錄的索引值
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// 驗證新增騎乘路線資訊內容
    /// </summary>
    public class AddRideRouteDataContentValidator : AbstractValidator<AddRideRouteDataContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public AddRideRouteDataContentValidator()
        {

        }
    }
}