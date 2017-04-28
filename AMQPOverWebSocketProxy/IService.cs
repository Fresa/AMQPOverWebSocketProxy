using System;
using Akka.Actor;
using Akka.DI.Core;

namespace AMQPOverWebSocketProxy
{
    public interface IService
    {
        void Start(Func<ActorSystem, IDependencyResolver> dependencyResolverFactory);
        void Stop();
    }
}