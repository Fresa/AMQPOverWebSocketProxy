using System;
using Akka.Actor;
using Common.Logging;
using Common.Logging.Loggers;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;

namespace AMQPOverWebSocketProxy.Actors
{
    public sealed class SocketActor : ReceiveActor
    {
        public class FailedStartingService { }

        private static ILogger Logger => new Lazy<ILogger>(LoggerFactory.Create<SocketActor>).Value;

        private readonly ILogFactory _logFactory;
        private readonly IBootstrap _socketBootstrap;

        public SocketActor(IBootstrap socketBootstrapper, ILogFactory logFactory)
        {
            _socketBootstrap = socketBootstrapper;
            _logFactory = logFactory;

            Start();
        }

        private void Start()
        {
            if (_socketBootstrap.Initialize(_logFactory) == false)
            {
                Logger.Fatal(null, "Failed to initialize socket server(s).");
                Context.Sender.Tell(new FailedStartingService());
                return;
            }

            var result = _socketBootstrap.Start();

            if (result == StartResult.Failed)
            {
                Logger.Fatal(null, "Failed to start socket server(s). Result: {0}", result);
                Context.Sender.Tell(new FailedStartingService());
                return;
            }

            Logger.Debug("Socket server(s) start sequence reported: {0}", result);
        }

        protected override void PostStop()
        {
            _socketBootstrap.Stop();
            base.PostStop();
        }
    }
}