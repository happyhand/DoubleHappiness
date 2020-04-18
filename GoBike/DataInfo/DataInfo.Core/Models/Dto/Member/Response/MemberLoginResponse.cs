using DataInfo.Core.Models.Dao.Member;

namespace DataInfo.Core.Models.Dto.Member.Response
{
    /// <summary>
    /// 會員登入後端回覆資料
    /// </summary>
    public class MemberLoginResponse
    {
        /// <summary>
        /// Gets or sets 註冊結果
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Gets or sets 會員資料
        /// </summary>
        public MemberDao LoginData { get; set; }
    }
}