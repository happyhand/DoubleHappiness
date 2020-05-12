using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
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
        protected readonly ILogger logger = LogManager.GetLogger("UploadService");

        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <param name="imgBase64s">imgBase64s</param>
        /// <param name="api">api</param>
        /// <returns>strings</returns>
        private async Task<IEnumerable<string>> UploadImages(IEnumerable<string> imgBase64s, string api)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.UploadServer.Domain, api, JsonConvert.SerializeObject(imgBase64s));
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("上傳圖片結果", $"Result: 上傳圖片失敗 ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", null);
                    return null;
                }

                string dataJson = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<string>>(dataJson);
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", ex);
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
                IEnumerable<string> imgUris = await this.UploadImages(imgBase64s, AppSettingHelper.Appsetting.UploadServer.Member.Api).ConfigureAwait(false);
                if (isIgnoreUri)
                {
                    imgUris = imgUris.Select(uri => uri.Replace(AppSettingHelper.Appsetting.UploadServer.Member.Uri, string.Empty));
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
                IEnumerable<string> imgUris = await this.UploadImages(imgBase64s, AppSettingHelper.Appsetting.UploadServer.Ride.Api).ConfigureAwait(false);
                if (isIgnoreUri)
                {
                    imgUris = imgUris.Select(uri => uri.Replace(AppSettingHelper.Appsetting.UploadServer.Ride.Uri, string.Empty));
                }

                return imgUris;
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳騎乘圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", ex);
                return null;
            }
        }
    }
}