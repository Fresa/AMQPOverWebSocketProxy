using AMQPOverWebSocketProxy.Actors.Messages;
using AMQPOverWebSocketProxy.Akka;
using Common.Serialization;
using RabbitMQ.Client;

namespace AMQPOverWebSocketProxy.Actors
{
    public class RabbitMqMessageProducerFactoryActor : ReceiveActor<IActorRef<IActorRef<Envelope>>>
    {
        private readonly IConnection _connection;
        private readonly ISerializer _serializer;

        public RabbitMqMessageProducerFactoryActor(IConnection connection, ISerializer serializer)
        {
            _connection = connection;
            _serializer = serializer;
        }

        protected override void OnReceive(IActorRef<IActorRef<Envelope>> actorReceivingActor)
        {
            var messageProducerActor = Context.ActorOf(Props<Envelope>.Create(
                () => new RabbitMqMessageProducerActor(_connection.CreateModel(), _serializer)));
            actorReceivingActor.Tell(messageProducerActor);
        }
    }
}