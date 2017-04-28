using System.Dynamic;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class JsonSubRequestInfo
    {
        public string Key { get; set; }
        public ExpandoObject Body { get; set; }
    }
}