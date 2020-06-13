namespace DataInfo.Core.Models.Dto.Team.View.data
{
    /// <summary>
    /// 路線資料可視資料
    /// </summary>
    public class RouteView
    {
        /// <summary>
        /// Gets or sets 路線座標
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets 路線座標
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets 路線備註
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets 路線圖片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets 路線地址
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// Gets or sets 路線鄉鎮市
        /// </summary>
        public string PlaceTitle { get; set; }
    }
}