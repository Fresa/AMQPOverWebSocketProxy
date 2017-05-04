using Akka.Actor;

namespace AMQPOverWebSocketProxy.Akka
{
    public abstract class UntypedActor<T> : UntypedActor
    {
        protected abstract void OnReceive(T message);

        protected override void OnReceive(object message)
        {
            if (message is T)
                OnReceive((T)message);
            else Unhandled(message);
        }
    }
}