using SuperSocket.SocketBase.Config;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public interface ISuperSocketConfigurationProvider
    {
        IConfigurationSource Get();
    }
}