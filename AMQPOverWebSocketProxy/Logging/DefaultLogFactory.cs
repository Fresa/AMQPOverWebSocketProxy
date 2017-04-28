using SuperSocket.SocketBase.Logging;

namespace AMQPOverWebSocketProxy.Logging
{
    public class DefaultLogFactory : ILogFactory
    {
        public ILog GetLog(string name)
        {
            return new Logger(name);
        }
    }
}