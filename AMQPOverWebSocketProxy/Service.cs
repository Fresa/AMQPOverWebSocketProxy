using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using AMQPOverWebSocketProxy.Actors;

namespace AMQPOverWebSocketProxy
{
    public sealed class Service : IService
    {
        private IDependencyResolver _resolver;
        private ActorSystem _actorSystem;

        public async void Start(Func<ActorSystem, IDependencyResolver> dependencyResolverFactory)
        {
            _actorSystem = ActorSystem.Create("AMQPOverWebSocketProxyActors");
            _resolver = dependencyResolverFactory(_actorSystem);

            _actorSystem.RegisterOnTermination(() =>
            {
                
            });

            var socketActor = _actorSystem.ActorOf(_resolver.Create<SocketActor>(), "socket-actor");



            try
            {
                await socketActor.Ask<SocketActor.FailedStartingService>(
                    "Did you fail starting the service?",
                    TimeSpan.FromSeconds(5));
            }
            catch (TaskCanceledException)
            {
                return;
            }
            Stop();            
        }

        public async void Stop()
        {
            await _actorSystem.Terminate();
        }
    }
}