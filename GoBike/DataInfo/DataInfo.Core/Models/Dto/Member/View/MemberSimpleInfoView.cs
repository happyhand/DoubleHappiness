using System;

namespace DataInfo.Core.Models.Dto.Member.View
{
    /// <summary>
    /// 會員簡易資訊可視資料
    /// </summary>
    public class MemberSimpleInfoView
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets OnlineType
        /// </summary>
        public int OnlineType { get; set; }
    }
}