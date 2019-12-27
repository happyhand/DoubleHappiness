using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MGT.Core.Resource;
using MGT.Repository.Interface;
using MGT.Repository.Models.Data;
using MGT.Service.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MGT.Service.Managers
{
    /// <summary>
    /// 後台服務
    /// </summary>
    public class MgtService : IMgtService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger<MgtService> logger;

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// mgtRepository
        /// </summary>
        private readonly IMgtRepository mgtRepository;

        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="mapper">mapper</param>
        /// <param name="mgtRepository">mgtRepository</param>
        /// <param name="redisRepository">redisRepository</param>
        public MgtService(ILogger<MgtService> logger, IMapper mapper, IMgtRepository mgtRepository, IRedisRepository redisRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.mgtRepository = mgtRepository;
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 新增代理商資料
        /// </summary>
        /// <returns>Tuple(AgentData, string)</returns>
        public void AddAgent(string account, string password)
        {
            try
            {
                this.mgtRepository.AddAgent(new AgentData() { Account = account, Password = password });
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Add Agent Error >>> Account:{account} Password:{password}\n{ex}");
            }
        }

        /// <summary>
        /// 代理商登入
        /// </summary>
        /// <param name="account">account</param>
        /// <param name="password">password</param>
        /// <returns>bool</returns>
        public async Task<string> AgentLogin(string account, string password)
        {
            try
            {
                this.logger.LogInformation($"Agent Login >>> Account:{account} Password:{password}");
                if (string.IsNullOrEmpty(account))
                {
                    return "代理商帳號無效.";
                }

                if (string.IsNullOrEmpty(password))
                {
                    return "代理商密碼無效.";
                }

                string cacheKey = $"{CommonFlagHelper.CommonFlag.RedisFlag.Agent}-{account}-{password}";
                string redisJsonData = this.redisRepository.GetCache(cacheKey);
                AgentData agentData = null;
                if (string.IsNullOrEmpty(redisJsonData))
                {
                    agentData = await this.mgtRepository.GetAgent(account, password);
                    this.redisRepository.SetCache(cacheKey, JsonConvert.SerializeObject(agentData), TimeSpan.FromMinutes(5));
                }
                else
                {
                    agentData = JsonConvert.DeserializeObject<AgentData>(redisJsonData);
                }

                if (agentData == null)
                {
                    return "無代理商資料.";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Agent Login Error >>> Account:{account} Password:{password}\n{ex}");
                return "代理商登入發生錯誤.";
            }
        }

        /// <summary>
        /// 取得代理商資料
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Tuple(AgentData, string)</returns>
        public async Task<Tuple<AgentData, string>> GetAgent(long id)
        {
            try
            {
                AgentData agentData = await this.mgtRepository.GetAgent(id);
                return Tuple.Create(agentData, string.Empty);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get Agent Error >>> ID:{id}\n{ex}");
                return Tuple.Create<AgentData, string>(null, "取得代理商資料發生錯誤.");
            }
        }
    }
}