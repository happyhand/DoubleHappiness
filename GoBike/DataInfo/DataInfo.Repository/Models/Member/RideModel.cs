using SqlSugar;
using System;

namespace DataInfo.Repository.Models.Member
{
    /// <summary>
    /// 騎乘資料
    /// </summary>
    public class RideModel
    {
        /// <summary>
        /// Gets or sets Altitude
        /// </summary>
        public decimal Altitude { get; set; }

        /// <summary>
        /// Gets or sets CountyID
        /// </summary>
        public int CountyID { get; set; }

        /// <summary>
        /// Gets or sets CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets Distance
        /// </summary>
        public decimal Distance { get; set; }

        /// <summary>
        /// Gets or sets Level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets Photo
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets RideID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string RideID { get; set; }

        /// <summary>
        /// Gets or sets Route
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets ShareContent
        /// </summary>
        public string ShareContent { get; set; }

        /// <summary>
        /// Gets or sets SharedType
        /// </summary>
        public int SharedType { get; set; }

        /// <summary>
        /// Gets or sets Time
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }
    }
}