using System;

namespace DataInfo.Core.Models.Dto.Member.View
{
    /// <summary>
    /// 會員名片資訊可視資料
    /// </summary>
    public class MemberCardInfoView
    {
        /// <summary>
        /// Gets or sets 頭像路徑
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 封面圖片路徑
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 是否有加入車隊
        /// </summary>
        public int HasTeam { get; set; }

        /// <summary>
        /// Gets or sets 暱稱
        /// </summary>
        public string Nickname { get; set; }
    }
}