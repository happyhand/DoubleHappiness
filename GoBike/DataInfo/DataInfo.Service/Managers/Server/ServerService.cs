﻿using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Server;
using DataInfo.Core.Models.Dto.Server;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Server
{
    /// <summary>
    /// 後端服務
    /// </summary>
    public class ServerService : IServerService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("ServerService");

        /// <summary>
        /// 接收後端回覆
        /// </summary>
        /// <param name="client">client</param>
        /// <returns>string</returns>
        private async Task<string> Receive(ClientWebSocket client)
        {
            try
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);
                WebSocketReceiveResult result = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    do
                    {
                        result = await client.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            return await reader.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("接收後端回覆發生錯誤", $"Client State: {client.State}", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 執行後端指令
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="commandID">commandID</param>
        /// <param name="commandType">commandType</param>
        /// <param name="data">data</param>
        /// <returns>CommandData(T)</returns>
        public async Task<CommandDto<T>> DoAction<T>(int commandID, string commandType, dynamic data)
        {
            try
            {
                using (ClientWebSocket client = new ClientWebSocket())
                {
                    //// 連線後端
                    await client.ConnectAsync(new Uri($"ws://{AppSettingHelper.Appsetting.CommandServer.ConnectionStrings}/{commandType}"), CancellationToken.None).ConfigureAwait(false);
                    //// 訂閱後端回覆
                    Task<string> result = this.Receive(client);
                    //// 發送後端指令
                    CommandDto<dynamic> commandData = new CommandDto<dynamic>() { CmdID = commandID, Data = data };
                    string sendDataJson = JsonConvert.SerializeObject(commandData);
                    ArraySegment<byte> array = new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendDataJson));
                    client.SendAsync(array, WebSocketMessageType.Text, true, CancellationToken.None);
                    //// 取得回應資訊
                    string receiveDataJson = await result.ConfigureAwait(false);
                    //// 關閉連線
                    //await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "1", CancellationToken.None).ConfigureAwait(false);
                    //client.Dispose();

                    return JsonConvert.DeserializeObject<CommandDto<T>>(receiveDataJson);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("執行後端指令發生錯誤", $"CommandID: {commandID} CommandType: {commandType} Data: {JsonConvert.SerializeObject(data)}", ex);
                return default;
            }
        }
    }
}