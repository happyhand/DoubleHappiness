﻿using DataInfo.Core.Models.Dto.Server;
using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Server
{
    /// <summary>
    /// 後端服務
    /// </summary>
    public interface IServerService
    {
        /// <summary>
        /// 執行後端指令
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="commandID">commandID</param>
        /// <param name="commandType">commandType</param>
        /// <param name="data">data</param>
        /// <returns>CommandData(T)</returns>
        Task<CommandDto<T>> DoAction<T>(int commandID, string commandType, dynamic data);
    }
}