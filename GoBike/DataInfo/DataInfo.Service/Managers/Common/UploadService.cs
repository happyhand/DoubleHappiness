using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dto.Common.Upload.Request;
using DataInfo.Service.Interfaces.Common;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Common
{
    /// <summary>
    /// 上傳服務
    /// </summary>
    public class UploadService : IUploadService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("UploadService");

        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="path">path</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        private async Task<IEnumerable<string>> UploadImages(IEnumerable<string> imgBase64s, string path, bool isIgnoreUri)
        {
            try
            {
                UploadImageRequest request = new UploadImageRequest()
                {
                    ImgBase64s = imgBase64s,
                    Path = path
                };

                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.UploadServer.Domain, AppSettingHelper.Appsetting.UploadServer.Api, JsonConvert.SerializeObject(request));
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("上傳圖片失敗", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)} Path: {path} IsIgnoreUri: {isIgnoreUri}", null);
                    return null;
                }

                string dataJson = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                IEnumerable<string> imgUris = JsonConvert.DeserializeObject<IEnumerable<string>>(dataJson);
                if (isIgnoreUri)
                {
                    imgUris = imgUris.Select(uri => uri.Replace(path, string.Empty));
                }

                return imgUris;
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)} Path: {path} IsIgnoreUri: {isIgnoreUri}", ex);
                return null;
            }
        }

        /// <summary>
        /// 上傳會員圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        public Task<IEnumerable<string>> UploadMemberImages(IEnumerable<string> imgBase64s, bool isIgnoreUri)
        {
            try
            {
                return this.UploadImages(imgBase64s, AppSettingHelper.Appsetting.UploadServer.Member.Path, isIgnoreUri);
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳會員圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)} IsIgnoreUri: {isIgnoreUri}", ex);
                return null;
            }
        }

        /// <summary>
        /// 上傳騎乘圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        public Task<IEnumerable<string>> UploadRideImages(IEnumerable<string> imgBase64s, bool isIgnoreUri)
        {
            try
            {
                return this.UploadImages(imgBase64s, AppSettingHelper.Appsetting.UploadServer.Ride.Path, isIgnoreUri);
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳騎乘圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)} IsIgnoreUri: {isIgnoreUri}", ex);
                return null;
            }
        }

        /// <summary>
        /// 上傳車隊活動圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        public Task<IEnumerable<string>> UploadTeamActivityImages(IEnumerable<string> imgBase64s, bool isIgnoreUri)
        {
            try
            {
                return this.UploadImages(imgBase64s, AppSettingHelper.Appsetting.UploadServer.TeamActivity.Path, isIgnoreUri);
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳車隊活動圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)} IsIgnoreUri: {isIgnoreUri}", ex);
                return null;
            }
        }

        /// <summary>
        /// 上傳車隊圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        public Task<IEnumerable<string>> UploadTeamImages(IEnumerable<string> imgBase64s, bool isIgnoreUri)
        {
            try
            {
                return this.UploadImages(imgBase64s, AppSettingHelper.Appsetting.UploadServer.Team.Path, isIgnoreUri);
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳車隊圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)} IsIgnoreUri: {isIgnoreUri}", ex);
                return null;
            }
        }
    }
}