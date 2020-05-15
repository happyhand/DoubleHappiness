using SqlSugar;
using System;

namespace DataInfo.Core.Models.Dao.Team.Table
{
    /// <summary>
    /// 車隊資料
    /// </summary>
    public class TeamData
    {
        /// <summary>
        /// Gets or sets 申請列表
        /// </summary>
        public string ApplyJoinList { get; set; }

        /// <summary>
        /// Gets or sets 車隊頭像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets 車隊所在地
        /// </summary>
        public int County { get; set; }

        /// <summary>
        /// Gets or sets 建立時間
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets 審核狀態(1:開，-1:關)
        /// </summary>
        public int ExamineStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊封面
        /// </summary>
        public string FrontCover { get; set; }

        /// <summary>
        /// Gets or sets 邀請列表
        /// </summary>
        public string InviteJoinList { get; set; }

        /// <summary>
        /// Gets or sets 車隊隊長 ID
        /// </summary>
        public string Leader { get; set; }

        /// <summary>
        /// Gets or sets 搜尋狀態(1:開，-1:關)
        /// </summary>
        public int SearchStatus { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string TeamID { get; set; }

        /// <summary>
        /// Gets or sets 車隊簡介
        /// </summary>
        public string TeamInfo { get; set; }

        /// <summary>
        /// Gets or sets 車隊隊員列表
        /// </summary>
        public string TeamMemberIDs { get; set; }

        /// <summary>
        /// Gets or sets 車隊名稱
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// Gets or sets 車隊副隊長列表
        /// </summary>
        public string TeamViceLeaderIDs { get; set; }
    }
}