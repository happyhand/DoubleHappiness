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
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets 身高
        /// </summary>
        public float BodyHeight { get; set; }

        /// <summary>
        /// Gets or sets 體重
        /// </summary>
        public float BodyWeight { get; set; }

        /// <summary>
        /// Gets or sets 封面圖片路徑
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 性別
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Gets or sets 是否已綁定手機
        /// </summary>
        public int HasMobile { get; set; }

        /// <summary>
        /// Gets or sets 手機
        /// </summary>
        public string Mobile { get; set; }
    }
}