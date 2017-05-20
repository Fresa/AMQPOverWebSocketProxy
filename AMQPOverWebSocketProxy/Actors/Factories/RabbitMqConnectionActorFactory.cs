using System.Linq;
using Akka.Actor;
using AMQPOverWebSocketProxy.Akka;
using Common.Serialization;
using RabbitMQ.Client;
using Envelope = AMQPOverWebSocketProxy.Actors.Messages.Envelope;

namespace AMQPOverWebSocketProxy.Actors.Factories
{
    public class RabbitMqConnectionActorFactory : IConnectionActorFactory
    {
        private readonly ISerializer _serializer;

        public RabbitMqConnectionActorFactory(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public IActorRef<IActorRef<IActorRef<Envelope>>> Connect(IUntypedActorContext context, IConnectSettings settings)
        {
            var factory = new ConnectionFactory
            {
                Port = settings.Port,
                UserName = settings.UserName,
                Password = settings.Password,
                VirtualHost = settings.VirtualHost
            };

            return context.ActorOf(Props<IActorRef<IActorRef<Envelope>>>.Create(() => new RabbitMqMessageProducerFactoryActor(factory.CreateConnection(settings.HostNames.ToList()), _serializer)));
        }
    }
}