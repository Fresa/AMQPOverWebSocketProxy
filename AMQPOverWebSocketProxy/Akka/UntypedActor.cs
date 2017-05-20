using System;
using Akka.Actor;

namespace AMQPOverWebSocketProxy.Akka
{
    public abstract class UntypedActor<T> : UntypedActor, IReceive<T>
    {
        protected abstract void OnReceive(T message);
        
        protected override void OnReceive(object message)
        {
            if (message is T)
                OnReceive((T)message);
            else Unhandled(message);
        }
    }

    public interface IReceive<T>
    {
    }

    public abstract class ReceiveActor<T> : ReceiveActor, IReceive<T>
    {
        protected ReceiveActor()
        {
            Receive<T>(message => OnReceive(message));
        }

        protected abstract void OnReceive(T message);
    }


}