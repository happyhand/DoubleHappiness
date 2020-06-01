using DataInfo.Core.Models.Dto.Member.View;
using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Team.View
{
    /// <summary>
    /// 車隊訊息可視資料
    /// </summary>
    public class TeamMessageView
    {
        /// <summary>
        /// Gets or sets 申請加入列表
        /// </summary>
        public IEnumerable<MemberSimpleInfoView> ApplyJoinList { get; set; }

        /// <summary>
        /// Gets or sets 公告列表
        /// </summary>
        public IEnumerable<TeamBullentiListView> BullentiList { get; set; }

        /// <summary>
        /// Gets or sets 邀請加入列表
        /// </summary>
        public IEnumerable<MemberSimpleInfoView> InviteJoinList { get; set; }

        /// <summary>
        /// Gets or sets 成員列表
        /// </summary>
        public IEnumerable<MemberSimpleInfoView> MemberList { get; set; }
    }
}