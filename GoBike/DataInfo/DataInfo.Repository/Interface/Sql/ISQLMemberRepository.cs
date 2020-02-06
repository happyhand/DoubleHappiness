﻿using DataInfo.Repository.Models.Data.Member;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataInfo.Repository.Interface.Sql
{
    /// <summary>
    /// 會員資料庫
    /// </summary>
    public interface ISQLMemberRepository
    {
        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="memberData">memberData</param>
        /// <returns>bool</returns>
        Task<bool> CreateMemberData(MemberData memberData);

        /// <summary>
        /// 取得會員資料 (By Email)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByEmail(string email);

        /// <summary>
        /// 取得會員資料 (By MemberID)
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>MemberData</returns>
        Task<MemberData> GetMemberDataByMemberID(string memberID);
    }
}