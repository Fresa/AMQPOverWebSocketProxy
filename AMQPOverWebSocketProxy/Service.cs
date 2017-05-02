using Akka.Actor;
using Akka.Configuration;
using AMQPOverWebSocketProxy.IOC;
using Topshelf;

namespace AMQPOverWebSocketProxy
{
    public sealed class Service : IService
    {
        private ActorSystem _actorSystem;

        public bool Start(HostControl hostControl, IDependencyResolverFactory dependencyResolverFactory)
        {
            _actorSystem = ActorSystem.Create("AMQP-over-WebSocket-Proxy-Actors");
            dependencyResolverFactory.Create(_actorSystem);

            return true;
        }

        public async void Stop()
        {
            await _actorSystem.Terminate();
        }
    }
}