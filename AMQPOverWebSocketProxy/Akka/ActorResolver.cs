using System;
using System.Collections.Generic;
using Akka.Actor;

namespace AMQPOverWebSocketProxy.Akka
{
    public class ActorResolver : IActorResolver
    {
        private readonly Dictionary<Type, Type> _register = new Dictionary<Type, Type>();

        public ActorResolver Register<TActorService, TActorImplementation>()
            where TActorService : ActorBase
            where TActorImplementation : TActorService
        {
            _register.Add(typeof(TActorService), typeof(TActorImplementation));
            return this;
        }

        public ActorResolver Register<TActorService, TActorImplementation, TMessage>()
            where TActorService : IReceive<TMessage>
            where TActorImplementation : TActorService
        {
            _register.Add(typeof(TActorService), typeof(TActorImplementation));
            return this;
        }

        public IActorRef Resolve<TService>(Func<Type, IActorRef> factory)
            where TService : ActorBase
        {
            return factory(_register[typeof(TService)]);
        }

        public IActorRef<TMessage> Resolve<TService, TMessage>(Func<Type, IActorRef<TMessage>> factory)
            where TService : ActorBase, IReceive<TMessage>
        {
            return factory(_register[typeof(TService)]);
        }
    }
}