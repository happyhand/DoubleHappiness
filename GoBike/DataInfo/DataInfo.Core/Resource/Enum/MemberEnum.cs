using System;
using System.Collections.Generic;
using System.Text;

namespace DataInfo.Core.Resource.Enum
{
    /// <summary>
    /// 性別類別資料
    /// </summary>
    public enum GenderType
    {
        /// <summary>
        /// 未設定
        /// </summary>
        None = 0,

        /// <summary>
        /// 女生
        /// </summary>
        Woman = 1,

        /// <summary>
        /// 男生
        /// </summary>
        Man = 2
    }

    /// <summary>
    /// 會員註冊來源類別資料
    /// </summary>
    public enum RegisterSourceType
    {
        /// <summary>
        /// 一般註冊
        /// </summary>
        Normal = 0,

        /// <summary>
        /// FB
        /// </summary>
        FB = 1,

        /// <summary>
        /// Google
        /// </summary>
        Google = 2
    }
}