using System;
using Akka.Actor;
using Akka.DI.Core;

namespace AMQPOverWebSocketProxy
{
    public interface IService
    {
        bool Start(Func<ActorSystem, IDependencyResolver> dependencyResolverFactory);
        bool Stop();
    }
}