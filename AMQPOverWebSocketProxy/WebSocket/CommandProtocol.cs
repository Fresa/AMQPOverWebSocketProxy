using System.Collections.Generic;
using System.Linq;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.WebSocket.Config;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class CommandProtocol : SubProtocolBase<WebSocketSession>
    {
        private readonly Dictionary<string, ISubCommand<WebSocketSession>> _commands;

        public CommandProtocol(IEnumerable<ISubCommand<WebSocketSession>> commands, IRequestInfoParser<SubRequestInfo> requestParser)
            : base("Json")
        {
            _commands = commands.ToDictionary(command => command.Name);
            SubRequestParser = requestParser;
        }

        public override bool Initialize(IAppServer appServer, SubProtocolConfig protocolConfig, ILog logger)
        {
            return true;
        }

        public override bool TryGetCommand(string name, out ISubCommand<WebSocketSession> command)
        {
            return _commands.TryGetValue(name, out command);
        }
    }
}