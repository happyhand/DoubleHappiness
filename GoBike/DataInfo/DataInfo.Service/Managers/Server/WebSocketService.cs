using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Server;
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
    /// WebSocket 服務
    /// </summary>
    public class WebSocketService : IWebSocketService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("WebSocketService");

        /// <summary>
        /// 建構式
        /// </summary>
        public WebSocketService()
        {
        }

        /// <summary>
        /// 接收訊息
        /// </summary>
        /// <param name="clientWebSocket">clientWebSocket</param>
        /// <returns>Task</returns>
        private async Task Receive(ClientWebSocket clientWebSocket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                        this.logger.LogInfo("接收訊息", $"Result: {result}", null);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        this.logger.LogInfo("接收訊息", $"Message: {await reader.ReadToEndAsync()}", null);
                    }
                }
            } while (true);
        }

        /// <summary>
        /// 開始連線
        /// </summary>
        /// <returns>Task</returns>
        public async Task Connect()
        {
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                try
                {
                    this.logger.LogInfo("開始連線", $"Host: 127.0.0.1 Port: {18591}", null);
                    await webSocket.ConnectAsync(new Uri("127.0.0.1"), CancellationToken.None);
                    await this.Receive(webSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR - {ex.Message}");
                }
            }
        }

        //public async Task Connect(HttpContext context, Func<Task> func)
        //{
        //    // 接受連線請求
        //    this.webSocket = await context.WebSockets.AcceptWebSocketAsync();

        //    // 當連線開啟時，持續接收訊息
        //    // 很重要: 就算後端只傳送訊息，也需要這段來保持連線開啟
        //    while (this.webSocket.State == WebSocketState.Open)
        //    {
        //        await this.webSocket.ReceiveAsync(new ArraySegment<byte>(new byte[1]), CancellationToken.None);
        //    }
        //}

        //public async Task SendAsync(dynamic data, CancellationToken cancellationToken)
        //{
        //    // 轉換data格式符合WebSocketWebSocket方法SendAsync的參數型態 轉換的方法不固定，可以自己改變方法
        //    var jsonStr = JsonConvert.SerializeObject(data);
        //    var bytes = Encoding.UTF8.GetBytes(jsonStr);
        //    var buffer = new ArraySegment<byte>(bytes);
        //    await this.webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        //}
    }
}