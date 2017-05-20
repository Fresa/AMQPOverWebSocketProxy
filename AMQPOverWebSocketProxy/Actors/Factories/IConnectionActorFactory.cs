using Akka.Actor;
using AMQPOverWebSocketProxy.Akka;
using Envelope = AMQPOverWebSocketProxy.Actors.Messages.Envelope;

namespace AMQPOverWebSocketProxy.Actors.Factories
{
    public interface IConnectionActorFactory
    {
        IActorRef<IActorRef<IActorRef<Envelope>>> Connect(IUntypedActorContext context, IConnectSettings settings);
    }
}