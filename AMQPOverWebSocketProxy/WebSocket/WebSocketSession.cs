using Common.Logging;
using Common.Logging.Loggers;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Protocol;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class WebSocketSession : WebSocketSession<WebSocketSession>
    {
        private new static readonly ILogger Logger = LoggerFactory.Create<WebSocketSession>();

        protected override void HandleUnknownCommand(SubRequestInfo requestInfo)
        {
        }

        protected override void HandleUnknownRequest(IWebSocketFragment requestInfo)
        {
            Logger.Debug($"Unknown request: {requestInfo.Key}");
            base.HandleUnknownRequest(requestInfo);
        }
    }
}