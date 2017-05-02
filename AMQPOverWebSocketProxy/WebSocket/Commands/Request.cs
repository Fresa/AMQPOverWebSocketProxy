using System.Collections.Generic;

namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    internal class Request<TBody>
    {
        public Dictionary<string, object> Headers { get; set; }
        public TBody Body { get; set; }
    }
}