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
        #region Messages

        public class FailedStartingService { }

        #endregion

        private static ILogger Logger => LoggerFactory.Create<SocketActor>();

        private readonly ILogFactory _logFactory;
        private readonly IBootstrap _socketBootstrap;

        public SocketActor(IBootstrap socketBootstrapper, ILogFactory logFactory)
        {
            _socketBootstrap = socketBootstrapper;
            _logFactory = logFactory;

            Start();
        }

        protected override void PostRestart(Exception reason)
        {
            Start();
            base.PostRestart(reason);
        }

        private void Start()
        {
            Logger.Info($"Starting up as {Self} by {Context.Parent}");
            if (_socketBootstrap.Initialize(_logFactory) == false)
            {
                Logger.Fatal(null, "Failed to initialize socket server(s).");
                Context.Parent.Tell(new FailedStartingService());
                return;
            }

            var result = _socketBootstrap.Start();

            if (result == StartResult.Failed)
            {
                Logger.Fatal(null, "Failed to start socket server(s). Result: {0}", result);
                Context.Parent.Tell(new FailedStartingService());
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

    public class TestActor : ReceiveActor
    {

        private static ILogger Logger => LoggerFactory.Create<TestActor>();

        public class TestMessage
        {
            public string Msg { get; set; }
        }

        public TestActor()
        {
            Logger.Info($"Starting up as {Self} by {Context.Parent}");
            Receive<TestMessage>(message =>
            {
                Logger.Info($"{Self}: Received {message.Msg} from {Sender}.");
                Sender.Tell(new SendActor.ReturnMessage { Msg = "Hello to you too!" });
            });
        }
    }

    public class SendActor : ReceiveActor
    {

        private static ILogger Logger => LoggerFactory.Create<TestActor>();

        public class ReturnMessage
        {
            public string Msg { get; set; }
        }

        public SendActor(IActorRef receiver)
        {
            Logger.Info($"Starting up as {Self} by {Context.Parent}");
            Receive<ReturnMessage>(message =>
            {
                Logger.Info($"{Self}: Received {message.Msg} from {Sender}.");
            });
            receiver.Tell(new TestActor.TestMessage { Msg = "Hello from the other side" });
        }
    }
}