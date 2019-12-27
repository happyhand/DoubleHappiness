using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MGT.Core.Resource
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
    }
}