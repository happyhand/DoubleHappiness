using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Interactive.Response
{
    /// <summary>
    /// 取得新增好友名單後端回覆資料
    /// </summary>
    public class GetNewFriendListResponse
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public string FriendList { get; set; }

        /// <summary>
        /// Gets or sets 回覆結果
        /// </summary>
        public int Result { get; set; }
    }
}