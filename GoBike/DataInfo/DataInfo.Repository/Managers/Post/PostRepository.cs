using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao.Post;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Repository.Interfaces.Post;
using DataInfo.Repository.Managers.Base;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Repository.Managers.Post
{
    /// <summary>
    /// 貼文資料庫
    /// </summary>
    public class PostRepository : MainBase, IPostRepository
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("PostRepository");

        /// <summary>
        /// redisRepository
        /// </summary>
        private readonly IRedisRepository redisRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="redisRepository">redisRepository</param>
        public PostRepository(IRedisRepository redisRepository)
        {
            this.redisRepository = redisRepository;
        }

        /// <summary>
        /// 取得貼文資料
        /// </summary>
        /// <param name="postIDs">postIDs</param>
        /// <returns>PostInfoDaos</returns>
        private async Task<IEnumerable<PostInfoDao>> GetInfoList(IEnumerable<string> postIDs)
        {
            try
            {
                if (postIDs == null)
                {
                    return new List<PostInfoDao>();
                }
                IEnumerable<string> cacheKeys = postIDs.Select(id => $"{AppSettingHelper.Appsetting.Redis.Flag.PostInfo}_{id}");
                Dictionary<string, PostInfoDao> postInfoDaoMap = await this.redisRepository.GetCache<PostInfoDao>(AppSettingHelper.Appsetting.Redis.PostDB, cacheKeys).ConfigureAwait(false);
                IEnumerable<PostInfoDao> values = postInfoDaoMap.Keys.Select(key =>
                {
                    PostInfoDao dao = postInfoDaoMap[key];
                    dao.PostID = key.Replace($"{AppSettingHelper.Appsetting.Redis.Flag.PostInfo}_", string.Empty);
                    return dao;
                });

                return values;
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得貼文資料發生錯誤", $"PostIDs: {JsonConvert.SerializeObject(postIDs)}", ex);
                return new List<PostInfoDao>();
            }
        }

        /// <summary>
        /// 取得會員的貼文列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>PostInfoDaos</returns>
        public async Task<IEnumerable<PostInfoDao>> GetMemberPostIDList(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.PostList}_{memberID}";
                IEnumerable<string> postIDs = await this.redisRepository.GetCache<IEnumerable<string>>(AppSettingHelper.Appsetting.Redis.PostDB, cacheKey).ConfigureAwait(false);
                return await this.GetInfoList(postIDs).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員的貼文ID列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<PostInfoDao>();
            }
        }

        /// <summary>
        /// 取得會員的塗鴉牆貼文列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>PostInfoDaos</returns>
        public async Task<IEnumerable<PostInfoDao>> GetMemberPostShowList(string memberID)
        {
            try
            {
                string cacheKey = $"{AppSettingHelper.Appsetting.Redis.Flag.PostShowList}_{memberID}";
                IEnumerable<string> postIDs = await this.redisRepository.GetCache<IEnumerable<string>>(AppSettingHelper.Appsetting.Redis.PostDB, cacheKey).ConfigureAwait(false);
                return await this.GetInfoList(postIDs).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員的塗鴉牆貼文列表發生錯誤", $"MemberID: {memberID}", ex);
                return new List<PostInfoDao>();
            }
        }
    }
}