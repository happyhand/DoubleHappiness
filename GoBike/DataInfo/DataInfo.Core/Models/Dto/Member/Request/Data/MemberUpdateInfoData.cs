using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Core.Models.Dto.Member.Request.Data
{
    /// <summary>
    /// 會員更新資訊資料
    /// </summary>
    public class MemberUpdateInfoData
    {
        /// <summary>
        /// Gets or sets 頭像
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
        /// Gets or sets 封面圖片
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 性別
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Gets or sets 手機
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets 首頁圖片
        /// </summary>
        public string Photo { get; set; }
    }
}