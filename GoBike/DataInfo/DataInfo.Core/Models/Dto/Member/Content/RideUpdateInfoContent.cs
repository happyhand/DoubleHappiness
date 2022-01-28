using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 更新騎乘資訊內容
    /// </summary>
    public class RideUpdateInfoContent
    {
        /// <summary>
        /// Gets or sets County
        /// </summary>
        public int County { get; set; }

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
        public int SharedType { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }
    }
}