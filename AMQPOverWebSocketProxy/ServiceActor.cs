using System;
using Akka.Actor;
using Akka.DI.Core;
using Common.Logging;
using Common.Logging.Loggers;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;

namespace AMQPOverWebSocketProxy
{
    public sealed class ServiceActor : ReceiveActor
    {
        private static ILogger Logger => new Lazy<ILogger>(LoggerFactory.Create<ServiceActor>).Value;

        private readonly ILogFactory _logFactory;
        private readonly IBootstrap _socketBootstrap;

        public ServiceActor(IBootstrap socketBootstrapper, ILogFactory logFactory)
        {
            _socketBootstrap = socketBootstrapper;
            _logFactory = logFactory;
            Start();
        }

        private void Start(Func<ActorSystem, IDependencyResolver> dependencyResolverFactory)
        {
            if (_socketBootstrap.Initialize(_logFactory) == false)
            {
                Logger.Fatal(null, "Failed to initialize socket server(s).");
                return false;
            }

            var result = _socketBootstrap.Start();

            if (result == StartResult.Failed)
            {
                Logger.Fatal(null, "Failed to start socket server(s). Result: {0}", result);
                return false;
            }

            Logger.Debug("Socket server(s) start sequence reported: {0}", result);
            return true;
        }

        private bool Stop()
        {
            _socketBootstrap.Stop();
            return true;
        }
    }

    public sealed class Service : IService
    {
        private IDependencyResolver _resolver;
        private readonly IActorRefFactory _actorFactory;
        private IActorRef _serviceActor;
        private ActorSystem _actorSystem;

        public Service(IDependencyResolver resolver, IActorRefFactory actorFactory)
        {
            _resolver = resolver;
            _actorFactory = actorFactory;
        }

        public bool Start(Func<ActorSystem, IDependencyResolver> dependencyResolverFactory)
        {
            _actorSystem = ActorSystem.Create("AMQPOverWebSocketProxyActors");
            _resolver = dependencyResolverFactory(_actorSystem);

            _serviceActor = _actorFactory.ActorOf(_resolver.Create<ServiceActor>(), "service-actor");
            return true;
        }

        public bool Stop()
        {
            return _actorSystem.Terminate().Wait(TimeSpan.FromSeconds(20));
        }
    }
}