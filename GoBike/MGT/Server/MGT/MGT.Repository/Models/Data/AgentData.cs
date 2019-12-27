using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGT.Repository.Models.Data
{
    /// <summary>
    /// 代理商資料
    /// </summary>
    public class AgentData
    {
        /// <summary>
        /// Gets or sets Account
        /// </summary>
        [Required]
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}