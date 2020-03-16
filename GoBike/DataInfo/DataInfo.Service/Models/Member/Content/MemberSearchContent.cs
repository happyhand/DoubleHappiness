using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Service.Models.Member.Content
{
    /// <summary>
    /// 搜尋會員內容
    /// </summary>
    public class MemberSearchContent
    {
        /// <summary>
        /// Gets or sets SearchKey
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// Gets or sets UseFuzzySearch
        /// </summary>
        public int UseFuzzySearch { get; set; }
    }
}