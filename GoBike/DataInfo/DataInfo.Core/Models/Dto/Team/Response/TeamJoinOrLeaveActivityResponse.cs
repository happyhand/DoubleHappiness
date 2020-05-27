﻿namespace DataInfo.Core.Models.Dto.Team.Response
{
    /// <summary>
    ///加入或離開車隊活動後端回覆資料
    /// </summary>
    public class TeamJoinOrLeaveActivityResponse
    {
        /// <summary>
        /// Gets or sets 更新動作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Gets or sets 回覆結果
        /// </summary>
        public int Result { get; set; }
    }
}