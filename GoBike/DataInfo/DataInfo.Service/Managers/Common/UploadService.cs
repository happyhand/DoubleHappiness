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
        /// <returns>strings</returns>
        private async Task<IEnumerable<string>> UploadImages(UploadImageRequest request)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.UploadServer.Domain, AppSettingHelper.Appsetting.UploadServer.Api, JsonConvert.SerializeObject(request));
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("上傳圖片結果", $"Result: 上傳圖片失敗 Request: {JsonConvert.SerializeObject(request)}", null);
                    return null;
                }

                string dataJson = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<string>>(dataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳圖片發生錯誤", $"Request: {JsonConvert.SerializeObject(request)}", ex);
                return null;
            }
        }

        /// <summary>
        /// 上傳會員圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        public async Task<IEnumerable<string>> UploadMemberImages(IEnumerable<string> imgBase64s, bool isIgnoreUri)
        {
            try
            {
                UploadImageRequest request = new UploadImageRequest()
                {
                    ImgBase64s = imgBase64s,
                    Path = AppSettingHelper.Appsetting.UploadServer.Member.Path
                };
                IEnumerable<string> imgUris = await this.UploadImages(request).ConfigureAwait(false);
                if (isIgnoreUri)
                {
                    imgUris = imgUris.Select(uri => uri.Replace(AppSettingHelper.Appsetting.UploadServer.Member.Path, string.Empty));
                }

                return imgUris;
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳會員圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", ex);
                return null;
            }
        }

        /// <summary>
        /// 上傳騎乘圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        public async Task<IEnumerable<string>> UploadRideImages(IEnumerable<string> imgBase64s, bool isIgnoreUri)
        {
            try
            {
                UploadImageRequest request = new UploadImageRequest()
                {
                    ImgBase64s = imgBase64s,
                    Path = AppSettingHelper.Appsetting.UploadServer.Ride.Path
                };
                IEnumerable<string> imgUris = await this.UploadImages(request).ConfigureAwait(false);
                if (isIgnoreUri)
                {
                    imgUris = imgUris.Select(uri => uri.Replace(AppSettingHelper.Appsetting.UploadServer.Ride.Path, string.Empty));
                }

                return imgUris;
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳騎乘圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", ex);
                return null;
            }
        }

        /// <summary>
        /// 上傳車隊活動圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        public async Task<IEnumerable<string>> UploadTeamActivityImages(IEnumerable<string> imgBase64s, bool isIgnoreUri)
        {
            try
            {
                UploadImageRequest request = new UploadImageRequest()
                {
                    ImgBase64s = imgBase64s,
                    Path = AppSettingHelper.Appsetting.UploadServer.TeamActivity.Path
                };
                IEnumerable<string> imgUris = await this.UploadImages(request).ConfigureAwait(false);
                if (isIgnoreUri)
                {
                    imgUris = imgUris.Select(uri => uri.Replace(AppSettingHelper.Appsetting.UploadServer.TeamActivity.Path, string.Empty));
                }

                return imgUris;
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳車隊活動圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", ex);
                return null;
            }
        }

        /// <summary>
        /// 上傳車隊圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="isIgnoreUri">isIgnoreUri</param>
        /// <returns>strings</returns>
        public async Task<IEnumerable<string>> UploadTeamImages(IEnumerable<string> imgBase64s, bool isIgnoreUri)
        {
            try
            {
                UploadImageRequest request = new UploadImageRequest()
                {
                    ImgBase64s = imgBase64s,
                    Path = AppSettingHelper.Appsetting.UploadServer.Team.Path
                };
                IEnumerable<string> imgUris = await this.UploadImages(request).ConfigureAwait(false);
                if (isIgnoreUri)
                {
                    imgUris = imgUris.Select(uri => uri.Replace(AppSettingHelper.Appsetting.UploadServer.Team.Path, string.Empty));
                }

                return imgUris;
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳車隊圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", ex);
                return null;
            }
        }
    }
}