using System;
using System.Collections.Generic;
using System.Text;
using MGT.Repository.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace MGT.Repository.Models.Context
{
    public class Maindb : DbContext
    {
        /// <summary>
        /// 主要DB
        /// </summary>
        /// <param name="options"></param>
        public Maindb(DbContextOptions<Maindb> options) : base(options)
        {
        }

        /// <summary>
        /// 代理商資料表
        /// </summary>
        public DbSet<MemberData> Member { get; set; }
    }
}