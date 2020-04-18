﻿using SqlSugar;
using System;

namespace DataInfo.Core.Models.Dao.Member
{
    /// <summary>
    /// 騎乘資料
    /// </summary>
    public class RideModel
    {
        /// <summary>
        /// Gets or sets 騎乘坡度
        /// </summary>
        public decimal Altitude { get; set; }

        /// <summary>
        /// Gets or sets 鄉鎮地區
        /// </summary>
        public int CountyID { get; set; }

        /// <summary>
        /// Gets or sets 建立日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets 騎乘距離
        /// </summary>
        public decimal Distance { get; set; }

        /// <summary>
        /// Gets or sets 等級
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 封面圖片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets RideID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string RideID { get; set; }

        /// <summary>
        /// Gets or sets 騎乘路線
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets 分享內容
        /// </summary>
        public string ShareContent { get; set; }

        /// <summary>
        /// Gets or sets 分享類型
        /// </summary>
        public int SharedType { get; set; }

        /// <summary>
        /// Gets or sets 騎乘時間
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Gets or sets 標題
        /// </summary>
        public string Title { get; set; }
    }
}