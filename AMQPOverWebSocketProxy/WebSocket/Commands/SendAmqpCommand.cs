using Akka.Actor;
using Common.Messaging.Queue.RabbitMQ.Producers;
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

    public class ConnectRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}