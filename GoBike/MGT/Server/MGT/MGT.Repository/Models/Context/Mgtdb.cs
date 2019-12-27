using System;
using System.Collections.Generic;
using System.Text;
using MGT.Repository.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace MGT.Repository.Models.Context
{
    public class Mgtdb : DbContext
    {
        /// <summary>
        /// 後台DB
        /// </summary>
        /// <param name="options"></param>
        public Mgtdb(DbContextOptions<Mgtdb> options) : base(options)
        {
        }

        /// <summary>
        /// 代理商資料表
        /// </summary>
        public DbSet<AgentData> Agent { get; set; }
    }
}