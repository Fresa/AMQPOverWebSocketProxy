using Akka.Actor;
using AMQPOverWebSocketProxy.Actors;
using AMQPOverWebSocketProxy.IOC;
using Topshelf;

namespace AMQPOverWebSocketProxy
{
    public sealed class Service : IService
    {
        private ActorSystem _actorSystem;

        public bool Start(HostControl hostControl, IDependencyResolverFactory dependencyResolverFactory)
        {
            _actorSystem = ActorSystem.Create("AMQPOverWebSocketProxyActors");
            dependencyResolverFactory.Create(_actorSystem);

            _actorSystem.ActorOf(Props.Create(() => new SocketSupervisor(hostControl.Stop)));
            return true;
        }

        public async void Stop()
        {
            await _actorSystem.Terminate();
        }
    }
}