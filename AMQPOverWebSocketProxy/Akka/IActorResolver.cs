using System;
using Akka.Actor;

namespace AMQPOverWebSocketProxy.Akka
{
    public interface IActorResolver
    {
        IActorRef Resolve<TService>(Func<Type, IActorRef> factory)
            where TService : ActorBase;

        IActorRef<TMessage> Resolve<TService, TMessage>(Func<Type, IActorRef<TMessage>> factory)
            where TService : ActorBase, IReceive<TMessage>;
    }
}