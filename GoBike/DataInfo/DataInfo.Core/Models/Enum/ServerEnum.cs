﻿namespace DataInfo.Core.Models.Enum
{
    /// <summary>
    /// 後端封包編號類別資料
    /// </summary>
    public enum CommandIDType
    {
        /// <summary>
        /// 使用者註冊
        /// </summary>
        UserRegistered = 1001,

        /// <summary>
        /// 使用者登入
        /// </summary>
        UserLogin = 1002,

        /// <summary>
        /// 更新使用者資訊
        /// </summary>
        UpdateUserInfo = 1003,
    }

    /// <summary>
    /// 後端封包類別資料
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// 測試
        /// </summary>
        test = 0,

        /// <summary>
        /// 會員
        /// </summary>
        User = 1
    }

    /// <summary>
    /// 使用者登入結果類別資料
    /// </summary>
    public enum UserLoginResultType
    {
        /// <summary>
        /// 登入失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 登入成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 帳號錯誤
        /// </summary>
        EmailError = 2,

        /// <summary>
        /// 密碼錯誤
        /// </summary>
        PasswordError = 3
    }

    /// <summary>
    /// 使用者註冊結果類別資料
    /// </summary>
    public enum UserRegisteredResultType
    {
        /// <summary>
        /// 註冊失敗
        /// </summary>
        Fail = 0,

        /// <summary>
        /// 註冊成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 帳號重複
        /// </summary>
        Repeat = 2,

        /// <summary>
        /// 密碼錯誤
        /// </summary>
        PasswordError = 3,

        /// <summary>
        /// 帳號格式不符
        /// </summary>
        EmailError = 4,
    }
}