using DataInfo.Service.Models.Member.Content.Interfaces;
using System.Collections.Generic;

namespace DataInfo.Service.Models.Member.Content
{
    /// <summary>
    /// 更新騎乘資訊內容
    /// </summary>
    public class RideUpdateInfoContent : IRideInfoContent
    {
        /// <summary>
        /// Gets or sets CountyID
        /// </summary>
        public int CountyID { get; set; }

        /// <summary>
        /// Gets or sets Level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets Photo
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets RideID
        /// </summary>
        public string RideID { get; set; }

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
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }
    }
}