using Akka.Actor;
using Common.Serialization;

namespace AMQPOverWebSocketProxy.WebSocket.Commands
{
    public class SendAmqpCommand : BaseCommand<AmqpRequest<object>>
    {
        public SendAmqpCommand(ISerializer serializer, IActorRefFactory actorRefFactory) : base(serializer, actorRefFactory)
        {
        }

        public override string Name { get; } = "Send";
    }
}