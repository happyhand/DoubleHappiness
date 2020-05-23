using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataInfo.Core.Models.Dao.Team
{
    /// <summary>
    /// 車隊活動資料
    /// </summary>
    public class TeamActivityDao
    {
        /// <summary>
        /// Gets or sets 活動日期
        /// </summary>
        public string ActDate { get; set; }

        /// <summary>
        /// Gets or sets 活動 ID
        /// </summary>
        public string ActID { get; set; }

        /// <summary>
        /// Gets or sets 建立日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets 路線描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets 主辦人頭像
        /// </summary>
        public string FounderAvatar { get; set; }

        /// <summary>
        /// Gets or sets 主辦人 ID
        /// </summary>
        public string FounderID { get; set; }

        /// <summary>
        /// Gets or sets 主辦人名稱
        /// </summary>
        public string FounderName { get; set; }

        /// <summary>
        /// Gets or sets 最高海拔
        /// </summary>
        public float MaxAltitude { get; set; }

        /// <summary>
        /// Gets or sets 集合時間
        /// </summary>
        public string MeetTime { get; set; }

        /// <summary>
        /// Gets or sets 參加成員列表
        /// </summary>
        public IEnumerable<string> MemberList { get { return JsonConvert.DeserializeObject<IEnumerable<string>>(this.MemberListtDataJson); } }

        /// <summary>
        /// Gets or sets 參加成員列表 Json
        /// </summary>
        public string MemberListtDataJson { get; set; }

        /// <summary>
        /// Gets or sets 路線
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets 車隊 ID
        /// </summary>
        public string TeamID { get; set; }

        /// <summary>
        /// Gets or sets 車隊名稱
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// Gets or sets 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets 總距離
        /// </summary>
        public float TotalDistance { get; set; }
    }
}