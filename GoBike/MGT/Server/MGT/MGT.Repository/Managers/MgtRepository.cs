using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MGT.Repository.Interface;
using MGT.Repository.Models.Context;
using MGT.Repository.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MGT.Repository.Managers
{
    /// <summary>
    /// 後台資料庫服務
    /// </summary>
    public class MgtRepository : IMgtRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger<MgtRepository> logger;

        /// <summary>
        /// mgtdb
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        public MgtRepository(ILogger<MgtRepository> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        #region 代理商

        /// <summary>
        /// 新增代理商資料
        /// </summary>
        /// <param name="agentData">agentData</param>
        public void AddAgent(AgentData agentData)
        {
            try
            {
                using (var mgtdb = new Mgtdb(this.serviceProvider.GetRequiredService<DbContextOptions<Mgtdb>>()))
                {
                    mgtdb.Agent.AddAsync(agentData);
                    mgtdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Add Agent Error >>> AgentData:{JsonConvert.SerializeObject(agentData)}\n{ex}");
            }
        }

        /// <summary>
        /// 取得代理商資料
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>AgentData</returns>
        public async Task<AgentData> GetAgent(long id)
        {
            try
            {
                using (var mgtdb = new Mgtdb(this.serviceProvider.GetRequiredService<DbContextOptions<Mgtdb>>()))
                {
                    return await mgtdb.Agent.FirstOrDefaultAsync(options => options.Id.Equals(id));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Agent Error >>> ID:{id}\n{ex}");
                return null;
            }
        }

        /// <summary>
        /// 取得代理商資料
        /// </summary>
        /// <param name="account">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns>AgentData</returns>
        public async Task<AgentData> GetAgent(string account, string password)
        {
            try
            {
                using (var mgtdb = new Mgtdb(this.serviceProvider.GetRequiredService<DbContextOptions<Mgtdb>>()))
                {
                    return await mgtdb.Agent.FirstOrDefaultAsync(options => options.Account.Equals(account) && options.Password.Equals(password));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Agent Error >>> Account:{account} Password:{password}\n{ex}");
                return null;
            }
        }

        #endregion 代理商

        #region 會員

        #region test

        /// <summary>
        /// 新增會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        public void AddMember(MemberData memberData)
        {
            try
            {
                using (var maindb = new Mgtdb(this.serviceProvider.GetRequiredService<DbContextOptions<Mgtdb>>()))
                {
                    maindb.Member.AddAsync(memberData);
                    maindb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Add Member Error >>> MemberData:{JsonConvert.SerializeObject(memberData)}\n{ex}");
            }
        }

        #endregion test

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="accountName">accountName</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberByAccountName(string accountName)
        {
            try
            {
                using (var maindb = new Mgtdb(this.serviceProvider.GetRequiredService<DbContextOptions<Mgtdb>>()))
                {
                    return await maindb.Member.FirstOrDefaultAsync(options => options.AccountName.Equals(accountName));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Member By AccountName Error >>> AccountName:{accountName}\n{ex}");
                return null;
            }
        }

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberByMemberID(int memberID)
        {
            try
            {
                using (var maindb = new Mgtdb(this.serviceProvider.GetRequiredService<DbContextOptions<Mgtdb>>()))
                {
                    return await maindb.Member.FirstOrDefaultAsync(options => options.Id.Equals(memberID));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Member By Member ID Error >>> MemberID:{memberID}\n{ex}");
                return null;
            }
        }

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="mobile">mobile</param>
        /// <returns>MemberData</returns>
        public async Task<MemberData> GetMemberByMobile(string mobile)
        {
            try
            {
                using (var maindb = new Mgtdb(this.serviceProvider.GetRequiredService<DbContextOptions<Mgtdb>>()))
                {
                    return await maindb.Member.FirstOrDefaultAsync(options => options.Mobile.Equals(mobile));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Member By Mobile Error >>> Mobile:{mobile}\n{ex}");
                return null;
            }
        }

        /// <summary>
        /// 取得所有會員資料
        /// </summary>
        /// <returns>MemberDatas</returns>
        public async Task<IEnumerable<MemberData>> GetMemberList()
        {
            try
            {
                using (var maindb = new Mgtdb(this.serviceProvider.GetRequiredService<DbContextOptions<Mgtdb>>()))
                {
                    return await maindb.Member.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Member List Error >>>\n{ex}");
                return null;
            }
        }

        #endregion 會員
    }
}