using System.Collections.Generic;

namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    public class SendCommand : BaseCommand<object>
    {
        protected override void Execute(WebSocketSession session, object message, IDictionary<string, object> headers)
        {

        }

        public override string Name { get; } = "Send";
    }
}