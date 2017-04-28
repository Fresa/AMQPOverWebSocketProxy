using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Sockets;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class WebSocketServer : WebSocketServer<WebSocketSession>
    {
        private readonly ILogFactory _loggerFactory;

        public WebSocketServer(
            ILogFactory loggerFactory,
            ISubProtocol<WebSocketSession> subProtocol,
            ISocketFactory socketFactory) : base(subProtocol, socketFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override ILog CreateLogger(string loggerName)
        {
            return _loggerFactory.GetLog(loggerName);
        }

        protected override void OnNewSessionConnected(WebSocketSession session)
        {
        }

        protected override void OnSessionClosed(WebSocketSession session, CloseReason reason)
        {
        }
    }
}