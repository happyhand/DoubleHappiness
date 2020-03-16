using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Server;
using NLog;

namespace DataInfo.Service.Managers.Server
{
    /// <summary>
    /// Socket 服務
    /// </summary>
    public class SocketService : ISocketService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("SocketService");

        /// <summary>
        /// 建構式
        /// </summary>
        public SocketService()
        {
        }

        private Socket ConnectSocket(string server, int port)
        {
            Socket s = null;

            // Get host related information.
            IPHostEntry hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address
            // family (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }

        // This method requests the home page content for the specified server.
        private string SocketSendReceive(string server, int port)
        {
            string request = "GET / HTTP/1.1\r\nHost: " + server +
                "\r\nConnection: Close\r\n\r\n";
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];
            string page = "";

            // Create a socket connection with the specified server and port.
            using (Socket s = ConnectSocket(server, port))
            {
                if (s == null)
                    return ("Connection failed");

                // Send request to the server.
                s.Send(bytesSent, bytesSent.Length, 0);

                // Receive the server home page content.
                int bytes = 0;
                page = "Default HTML page on " + server + ":\r\n";

                // The following will block until the page is transmitted.
                do
                {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    page += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                    this.logger.LogInfo("Socket 服務 running", $"Page: {page}", null);
                }
                while (bytes > 0);
            }

            return page;
        }

        public void Start()
        {
            try
            {
                string result = SocketSendReceive("127.0.0.1", 18591);
                this.logger.LogInfo("Socket 服務 start", $"Result: {result}", null);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Socket 服務 Error", string.Empty, ex);
            }
        }
    }
}