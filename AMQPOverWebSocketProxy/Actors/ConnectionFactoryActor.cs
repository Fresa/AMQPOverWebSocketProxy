using System.Collections.Generic;
using System.Linq;
using AMQPOverWebSocketProxy.Actors.Factories;
using AMQPOverWebSocketProxy.Actors.Messages;
using AMQPOverWebSocketProxy.Akka;

namespace AMQPOverWebSocketProxy.Actors
{
    public class ConnectionFactoryActor : ReceiveActor<ConnectionFactoryActor.Connect>
    {
        #region Messages
        public class Connect
        {
            public Connect(VirtualConnectionSettings connectionSettings, IActorRef<IActorRef<Envelope>> messageProducerReceivingActorRef)
            {
                ConnectionSettings = connectionSettings;
                MessageProducerReceivingActorRef = messageProducerReceivingActorRef;
            }

            public VirtualConnectionSettings ConnectionSettings { get; }
            public IActorRef<IActorRef<Envelope>> MessageProducerReceivingActorRef { get; }
        }

        public class VirtualConnectionSettings
        {
            public VirtualConnectionSettings(string name, string password, string virtualHost)
            {
                Name = name;
                Password = password;
                VirtualHost = virtualHost;
            }

            public string VirtualHost { get; }
            public string Name { get; }
            public string Password { get; }
        }


        public class HostSettings
        {
            public HostSettings(IEnumerable<string> hostNames, int port)
            {
                HostNames = hostNames.ToList();
                Port = port;
            }

            public IReadOnlyList<string> HostNames { get; }
            public int Port { get; }
        }

        #endregion

        private readonly IConnectionActorFactory _connectionFactory;
        private readonly HostSettings _settings;

        public ConnectionFactoryActor(IConnectionActorFactory connectionFactory, HostSettings settings)
        {
            _connectionFactory = connectionFactory;
            _settings = settings;
        }

        protected override void OnReceive(Connect connect)
        {
            var connectionActor = _connectionFactory.Connect(Context, new ConnectionSettings(connect.ConnectionSettings.Name, connect.ConnectionSettings.Password,
                connect.ConnectionSettings.VirtualHost, _settings.Port, _settings.HostNames));

            connectionActor.Tell(connect.MessageProducerReceivingActorRef);
        }
    }
}