using Akka.Actor;

namespace AMQPOverWebSocketProxy.Akka
{
    public interface IActorRef<in T> : IActorRef
    {
        void Tell(T message, IActorRef sender);
        void Tell(T message);
    }
}