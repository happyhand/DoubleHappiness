using SqlSugar;
using System;

namespace DataInfo.Core.Models.Dao.Member
{
    /// <summary>
    /// 會員資料
    /// </summary>
    public class MemberDao
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

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
        /// Gets or sets 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets FB認證碼
        /// </summary>
        public string FBToken { get; set; }

        /// <summary>
        /// Gets or sets 封面路徑
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 性別
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Gets or sets Google認證碼
        /// </summary>
        public string GoogleToken { get; set; }

        /// <summary>
        /// Gets or sets 登入時間
        /// </summary>
        public DateTime? LoginDate { get; set; }

        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 手機
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets 註冊日期
        /// </summary>
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// Gets or sets 註冊來源
        /// </summary>
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