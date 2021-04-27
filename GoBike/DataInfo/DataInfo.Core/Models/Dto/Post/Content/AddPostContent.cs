using System.Collections.Generic;

namespace DataInfo.Core.Models.Dto.Post.Content
{
    /// <summary>
    /// 新增貼文內容
    /// </summary>
    public class AddPostContent
    {
        /// <summary>
        /// Gets or sets Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets Photos
        /// </summary>
        public IEnumerable<string> Photo { get; set; }
    }
}