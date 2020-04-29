using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Core.Models.Dto.Response;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
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
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> UploadImages(IEnumerable<string> imgBase64s)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await Utility.ApiPost(AppSettingHelper.Appsetting.UploadServer.Domain, AppSettingHelper.Appsetting.UploadServer.ImageApi, JsonConvert.SerializeObject(imgBase64s));
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogWarn("上傳圖片結果", $"Result: 上傳圖片失敗 ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "上傳圖片失敗."
                    };
                }

                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = JsonConvert.DeserializeObject<IEnumerable<string>>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false))
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳圖片發生錯誤", $"ImgBase64s: {JsonConvert.SerializeObject(imgBase64s)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "上傳圖片發生錯誤."
                };
            }
        }
    }
}