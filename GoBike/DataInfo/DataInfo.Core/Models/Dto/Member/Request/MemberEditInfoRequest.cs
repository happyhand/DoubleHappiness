using DataInfo.Core.Models.Dto.Member.Request.Data;

namespace DataInfo.Core.Models.Dto.Member.Request
{
    /// <summary>
    /// 會員編輯資訊請求後端資料
    /// </summary>
    public class MemberEditInfoRequest
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 更新資料
        /// </summary>
        public MemberUpdateInfoData UpdateData { get; set; }
    }
}