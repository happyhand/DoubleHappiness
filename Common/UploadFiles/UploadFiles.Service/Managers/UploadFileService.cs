using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadFiles.Core.Applibs;
using UploadFiles.Core.Extensions;
using UploadFiles.Core.Models.Dto.Image.Content;
using UploadFiles.Service.Interfaces;

namespace UploadFiles.Service.Managers
{
    /// <summary>
    /// 檔案上傳服務
    /// </summary>
    public class UploadFileService : IUploadFileService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("UploadFileService");

        /// <summary>
        /// 建構式
        /// </summary>
        public UploadFileService()
        {
        }

        /// <summary>
        /// 取得新的檔案名稱
        /// </summary>
        /// <param name="fileExtensionName">fileExtensionName</param>
        /// <returns>string</returns>
        private string GetNewFileName(string fileExtensionName)
        {
            string guid = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 6);
            return $"{guid}-{DateTime.UtcNow:yyyyMMdd-HHmmss}{fileExtensionName}";
        }

        /// <summary>
        /// 上傳圖像
        /// </summary>
        /// <param name="httpRequest">httpRequest</param>
        /// <param name="projectName">projectName</param>
        /// <param name="typeName">typeName</param>
        /// <returns>strings</returns>
        public async Task<IEnumerable<string>> UploadImages(HttpRequest httpRequest, string projectName, string typeName)
        {
            try
            {
                List<string> fileUrls = new List<string>();
                FormValueProvider formValueProvider = await httpRequest.StreamFile((file) =>
                {
                    this.logger.LogInfo("請求上傳圖像", $"ProjectName: {projectName} TypeName: {typeName} File: {file.FileName}", null);
                    string fileExtensionName = Path.GetExtension(file.FileName);
                    string fileName = this.GetNewFileName(fileExtensionName);
                    string fileUrl = $"{projectName}/images/{typeName}/{DateTime.UtcNow:yyyyMMdd}/{fileName}";
                    string filePath = ($"{AppSettingHelper.Appsetting.CdnPath}/{fileUrl}").ToLower();
                    string fileDirectoryName = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(fileDirectoryName))
                    {
                        Directory.CreateDirectory(fileDirectoryName);
                    }

                    fileUrls.Add(fileUrl);
                    return File.Create(filePath);
                });

                this.logger.LogInfo("上傳圖像完成", $"Files: {JsonConvert.SerializeObject(fileUrls)}", null);
                return fileUrls;
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳圖像發生錯誤", $"ProjectName: {projectName} TypeName: {typeName}", ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// 上傳圖像
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>imgUrls</returns>
        public IEnumerable<string> UploadImages(ImageContent content)
        {
            try
            {
                IEnumerable<string> imgBase64s = content.ImgBase64s;
                string cdnPath = AppSettingHelper.Appsetting.CdnPath;
                string fileDirectoryName = $"{cdnPath}/{content.Project}/images/{content.Type}/{DateTime.UtcNow:yyyyMMdd}/";
                if (!Directory.Exists(fileDirectoryName))
                {
                    Directory.CreateDirectory(fileDirectoryName);
                }

                int noOfImgBase64s = imgBase64s.Count();
                List<string> imgUrls = new List<string>();
                for (int i = 0; i < noOfImgBase64s; i++)
                {
                    string imgBase64 = imgBase64s.ElementAt(i);
                    if (string.IsNullOrEmpty(imgBase64))
                    {
                        imgUrls.Add(string.Empty);
                        continue;
                    }

                    int commaIndex = imgBase64.IndexOf(",");
                    var bytes = Convert.FromBase64String(commaIndex == -1 ? imgBase64 : imgBase64.Substring(imgBase64.IndexOf(",") + 1));
                    string fileName = this.GetNewFileName(".png");
                    string filePath = ($"{fileDirectoryName}{fileName}").ToLower();
                    using (FileStream imageFile = new FileStream(filePath, FileMode.Create))
                    {
                        try
                        {
                            imageFile.Write(bytes, 0, bytes.Length);
                            imageFile.Flush();
                            imgUrls.Add(filePath.Replace(cdnPath.ToLower(), string.Empty));
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogError("上傳圖像發生錯誤", $"FilePath: {filePath}", ex);
                            imgUrls.Add(string.Empty);
                        }
                    }
                }

                this.logger.LogInfo("上傳圖像完成", $"Files: {JsonConvert.SerializeObject(imgUrls)}", null);
                return imgUrls;
            }
            catch (Exception ex)
            {
                this.logger.LogError("上傳圖像發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new List<string>();
            }
        }
    }
}