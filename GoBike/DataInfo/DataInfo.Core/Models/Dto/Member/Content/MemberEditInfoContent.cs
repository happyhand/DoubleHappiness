﻿using System;

namespace DataInfo.Core.Models.Dto.Member.Content
{
    /// <summary>
    /// 會員編輯資訊內容
    /// </summary>
    public class MemberEditInfoContent
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 生日
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets 身高
        /// </summary>
        public double BodyHeight { get; set; }

        /// <summary>
        /// Gets or sets 體重
        /// </summary>
        public double BodyWeight { get; set; }

        /// <summary>
        /// Gets or sets 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets 封面圖片路徑
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 性別
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets 首頁圖片路徑
        /// </summary>
        public string Photo { get; set; }
    }
}