using System;
using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Ride.View
{
    /// <summary>
    /// 騎乘路線可視資料
    /// </summary>
    public class RideRouteView
    {
        /// <summary>
        /// Gets or sets 路線資訊
        /// </summary>
        public IEnumerable<IEnumerable<string>> Route { get; set; }
    }
}