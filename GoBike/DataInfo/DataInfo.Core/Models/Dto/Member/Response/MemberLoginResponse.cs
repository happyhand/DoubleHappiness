using DataInfo.Core.Models.Dao.Member;

namespace DataInfo.Core.Models.Dto.Member.Response
{
    /// <summary>
    /// 會員登入後端回覆資料
    /// </summary>
    public class MemberLoginResponse
    {
        /// <summary>
        /// Gets or sets 會員 ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// Gets or sets 回覆結果
        /// </summary>
        public int Result { get; set; }
    }
}