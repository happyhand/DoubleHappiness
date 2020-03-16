using System.Collections.Generic;

namespace DataInfo.Service.Models.Member.Content.Interfaces
{
    public interface IRideInfoContent
    {
        /// <summary>
        /// Gets or sets CountyID
        /// </summary>
        int CountyID { get; set; }

        /// <summary>
        /// Gets or sets Level
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// Gets or sets Photo
        /// </summary>
        string Photo { get; set; }

        /// <summary>
        /// Gets or sets Route
        /// </summary>
        IEnumerable<IEnumerable<string>> Route { get; set; }

        /// <summary>
        /// Gets or sets ShareContent
        /// </summary>
        IEnumerable<IEnumerable<string>> ShareContent { get; set; }

        /// <summary>
        /// Gets or sets SharedType
        /// </summary>
        int SharedType { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        string Title { get; set; }
    }
}