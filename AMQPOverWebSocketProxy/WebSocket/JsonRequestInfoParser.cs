using Common.Serialization;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class JsonRequestInfoParser : IRequestInfoParser<SubRequestInfo>
    {
        private readonly ISerializer _serializer;

        public JsonRequestInfoParser(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public SubRequestInfo ParseRequestInfo(string source)
        {
            var request = _serializer.DeserializeFromString<JsonSubRequestInfo>(source);
            return new SubRequestInfo(request.Key, "", _serializer.SerializeToString(request.Body));
        }
    }
}