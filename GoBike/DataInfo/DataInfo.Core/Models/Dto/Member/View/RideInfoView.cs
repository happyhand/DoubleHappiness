using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Member.View
{
    /// <summary>
    /// 騎乘資訊回應客端資料
    /// </summary>
    public class RideInfoView
    {
        /// <summary>
        /// Gets or sets Altitude
        /// </summary>
        public string Altitude { get; set; }

        /// <summary>
        /// Gets or sets County
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Gets or sets Distance
        /// </summary>
        public string Distance { get; set; }

        /// <summary>
        /// Gets or sets Photo
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets RideID
        /// </summary>
        public string RideID { get; set; }

        /// <summary>
        /// Gets or sets ShareContent
        /// </summary>
        public IEnumerable<IEnumerable<string>> ShareContent { get; set; }

        /// <summary>
        /// Gets or sets SharedType
        /// </summary>
        public string SharedType { get; set; }

        /// <summary>
        /// Gets or sets Time
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }
    }
}