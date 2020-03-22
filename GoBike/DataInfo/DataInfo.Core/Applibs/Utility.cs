using DataInfo.Core.Extensions;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataInfo.Core.Applibs
{
    /// <summary>
    /// 共用方法
    /// </summary>
    public class Utility
    {
        #region AES 加解密功能

        /// <summary>
        /// byte 轉 16 進制
        /// </summary>
        /// <param name="comByte">comByte</param>
        /// <returns>string</returns>
        public static string ByteToHex(byte[] comByte)
        {
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            foreach (byte data in comByte)
            {
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            return builder.ToString().ToUpper().Replace(" ", string.Empty);
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>string</returns>
        public static string DecryptAES(string text)
        {
            //byte[] encryptBytes = Convert.FromBase64String(text);
            byte[] encryptBytes = HexToByte(text);
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(AppSettingHelper.Appsetting.Aes.Key);
            aes.IV = Encoding.UTF8.GetBytes(AppSettingHelper.Appsetting.Aes.Iv);
            ICryptoTransform cryptoTransform = aes.CreateDecryptor();
            byte[] bResult = cryptoTransform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length);
            return Encoding.UTF8.GetString(bResult);
        }

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>string</returns>
        public static string EncryptAES(string text)
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(text);
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(AppSettingHelper.Appsetting.Aes.Key);
            aes.IV = Encoding.UTF8.GetBytes(AppSettingHelper.Appsetting.Aes.Iv);
            ICryptoTransform cryptoTransform = aes.CreateEncryptor();
            byte[] bResult = cryptoTransform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
            return ByteToHex(bResult);
            //return Convert.ToBase64String(bResult);
        }

        /// <summary>
        /// 16 進制轉 byte
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>byte[]</returns>
        public static byte[] HexToByte(string data)
        {
            data = data.Replace(" ", string.Empty);
            byte[] comBuffer = new byte[data.Length / 2];
            for (int i = 0; i < data.Length; i += 2)
            {
                comBuffer[i / 2] = (byte)Convert.ToByte(data.Substring(i, 2), 16);
            }

            return comBuffer;
        }

        #endregion AES 加解密功能

        #region API 串接

        /// <summary>
        /// API Get
        /// </summary>
        /// <param name="domain">domain</param>
        /// <param name="apiUrl">apiUrl</param>
        /// <returns>HttpResponseMessage</returns>
        public static async Task<HttpResponseMessage> ApiGet(string domain, string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://{domain}/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return await client.GetAsync(apiUrl).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// API POST
        /// </summary>
        /// <param name="domain">domain</param>
        /// <param name="apiUrl">apiUrl</param>
        /// <param name="postData">postData</param>
        /// <returns>HttpResponseMessage</returns>
        public static async Task<HttpResponseMessage> ApiPost(string domain, string apiUrl, string postData)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://{domain}/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");
                return await client.PostAsync(apiUrl, content).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// API POST
        /// </summary>
        /// <param name="domain">domain</param>
        /// <param name="apiUrl">apiUrl</param>
        /// <param name="files">files</param>
        /// <returns>HttpResponseMessage</returns>
        public static async Task<HttpResponseMessage> ApiPost(string domain, string apiUrl, IFormFileCollection files)
        {
            /// 建立 HttpClient
            using (HttpClient client = new HttpClient())
            {
                /// 設定站台 url (api url)
                client.BaseAddress = new Uri($"http://{domain}/");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.LogInfo("ApiPost", $"Path: {$"http://{domain}/"}", null);
                /// 讀取 Request 中的檔案，並轉換成 byte 型式
                byte[] dataBytes;
                MultipartFormDataContent multiContent = new MultipartFormDataContent();
                foreach (IFormFile file in files)
                {
                    using (BinaryReader binaryReader = new BinaryReader(file.OpenReadStream()))
                    {
                        dataBytes = binaryReader.ReadBytes((int)file.OpenReadStream().Length);
                        ByteArrayContent bytes = new ByteArrayContent(dataBytes);
                        multiContent.Add(bytes, "file", file.FileName);
                    }
                }
                /// 呼叫 api 並接收回應
                return await client.PostAsync(apiUrl, multiContent).ConfigureAwait(false);
            }
        }

        #endregion API 串接

        #region 流水號

        /// <summary>
        /// 取得流水號 ID
        /// </summary>
        /// <param name="createDate">createDate</param>
        /// <returns>string</returns>
        public static string GetSerialID(DateTime createDate)
        {
            return $"{Guid.NewGuid().ToString().Substring(0, 6)}-{createDate:yyyy}-{createDate:MMdd}";
        }

        #endregion 流水號

        #region 驗證功能

        /// <summary>
        /// Email 格式驗證
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>bool</returns>
        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        #endregion 驗證功能
    }
}