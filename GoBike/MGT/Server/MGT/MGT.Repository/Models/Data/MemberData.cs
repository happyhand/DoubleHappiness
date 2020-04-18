﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGT.Repository.Models.Data
{
    /// <summary>
    /// 會員資料
    /// </summary>
    public class MemberData
    {
        /// <summary>
        /// Gets or sets 帳號
        /// </summary>
        [Required]
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// Gets or sets 身高
        /// </summary>
        public double BodyHeight { get; set; }

        /// <summary>
        /// Gets or sets 體重
        /// </summary>
        public double BodyWeight { get; set; }

        /// <summary>
        /// Gets or sets 居住地
        /// </summary>
        public int Country { get; set; }

        /// <summary>
        /// Gets or sets FB認證碼
        /// </summary>
        public string FBToken { get; set; }

        /// <summary>
        /// Gets or sets 封面路徑
        /// </summary>
        public string FrontCoverUrl { get; set; }

        /// <summary>
        /// Gets or sets 性別
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Gets or sets Google認證碼
        /// </summary>
        public string GoogleToken { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets 登入時間
        /// </summary>
        public DateTime LoginDate { get; set; }

        /// <summary>
        /// Gets or sets 手機
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets 密碼
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets 註冊日期
        /// </summary>
        [Required]
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// Gets or sets 註冊來源
        /// </summary>
        [Required]
        public int RegisterSource { get; set; }

        /// <summary>
        /// Gets or sets 總騎乘爬升
        /// </summary>
        public double TotalAltitude { get; set; }

        /// <summary>
        /// Gets or sets 總騎乘距離
        /// </summary>
        public double TotalDistance { get; set; }

        /// <summary>
        /// Gets or sets 總騎乘時間
        /// </summary>
        public long TotalRideTime { get; set; }
    }
}