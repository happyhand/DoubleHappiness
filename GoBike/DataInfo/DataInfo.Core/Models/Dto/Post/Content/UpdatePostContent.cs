using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Post.Content
{
    /// <summary>
    /// 更新貼文內容
    /// </summary>
    public class UpdatePostContent
    {
        /// <summary>
        /// Gets or sets Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets Photos
        /// </summary>
        public IEnumerable<string> Photo { get; set; }

        /// <summary>
        /// Gets or sets PostID
        /// </summary>
        public string PostID { get; set; }
    }
}