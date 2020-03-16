using System.Threading.Tasks;

namespace DataInfo.Service.Interfaces.Server
{
    /// <summary>
    /// WebSocket 服務
    /// </summary>
    public interface IWebSocketService
    {
        Task Connect();
    }
}