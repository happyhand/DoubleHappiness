using System;

namespace DataInfo.Core.Models.Dto.Member.View
{
    /// <summary>
    /// 會員詳細資訊可視資料
    /// </summary>
    public class MemberDetailInfoView : MemberSimpleInfoView
    {
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
        /// Gets or sets 封面路徑
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
    }
}