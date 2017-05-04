using System.Collections.Generic;

namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    public class AmqpRequest<TBody>
    {
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public string MessageKey { get; set; }
        public string Exchange { get; set; }

        public Dictionary<string, object> Headers { get; set; }
        public TBody Body { get; set; }
    }
}