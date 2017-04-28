using System;
using Akka.Actor;
using Akka.DI.Core;
using AMQPOverWebSocketProxy.Actors;

namespace AMQPOverWebSocketProxy
{
    public sealed class Service : IService
    {
        private IDependencyResolver _resolver;
        private ActorSystem _actorSystem;

        public bool Start(Func<ActorSystem, IDependencyResolver> dependencyResolverFactory)
        {
            _actorSystem = ActorSystem.Create("AMQPOverWebSocketProxyActors");
            _resolver = dependencyResolverFactory(_actorSystem);

            _actorSystem.ActorOf(_resolver.Create<SocketActor>(), "service-actor");
            return true;
        }

        public bool Stop()
        {
            return _actorSystem.Terminate().Wait(TimeSpan.FromSeconds(20));
        }
    }
}