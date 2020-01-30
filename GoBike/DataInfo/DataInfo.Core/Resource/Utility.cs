using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataInfo.Core.Resource
{
    /// <summary>
    /// 共用方法
    /// </summary>
    public class Utility
    {
        #region AES 加解密功能

        /// <summary>
        /// AES_IV
        /// </summary>
        public static string AES_IV = "2244668800113355";

        /// <summary>
        /// AES_KEY
        /// </summary>
        public static string AES_KEY = "1133557799224466";

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
            aes.Key = Encoding.UTF8.GetBytes(AES_KEY);
            aes.IV = Encoding.UTF8.GetBytes(AES_IV);
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
            aes.Key = Encoding.UTF8.GetBytes(AES_KEY);
            aes.IV = Encoding.UTF8.GetBytes(AES_IV);
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
                return await client.GetAsync(apiUrl);
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
                return await client.PostAsync(apiUrl, content);
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
            //// 建立 HttpClient
            using (HttpClient client = new HttpClient())
            {
                //// 設定站台 url (api url)
                client.BaseAddress = new Uri($"http://{domain}/");
                //// 讀取 Request 中的檔案，並轉換成 byte 型式
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
                //// 呼叫 api 並接收回應
                return await client.PostAsync(apiUrl, multiContent);
            }
        }

        #endregion API 串接
    }
}