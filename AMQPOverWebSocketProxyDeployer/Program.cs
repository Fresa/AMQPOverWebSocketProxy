using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using AMQPOverWebSocketProxy;
using AMQPOverWebSocketProxy.Actors;
using AMQPOverWebSocketProxy.IOC;
using AMQPOverWebSocketProxy.Logging;
using AMQPOverWebSocketProxy.Serialization;
using AMQPOverWebSocketProxy.WebSocket;
using Common.Serialization;
using Common.Serialization.Serializer;
using Newtonsoft.Json;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Sockets;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxyDeployer
{
    class Program
    {
        private static readonly Container Container = new Container();

        static void Main(string[] args)
        {
            Configure();

            using (var system = ActorSystem.Create("AMQP-over-WebSocket-Proxy-Actors-Deployer"))
            {
                var dependencyResolverFactory = new SimpleInjectorDependencyResolverFactory(Container);
                dependencyResolverFactory.Create(system);

                system.ActorOf(Props.Create(() => new SocketSupervisorActor()), "socket-supervisor-actor");

                Console.ReadKey();
            }
        }

        private static void Configure()
        {
            /* WebSocket */
            Container.RegisterSingleton<IBootstrap, SuperSocketBootStrapper>();
            Container.RegisterSingleton<ISuperSocketConfigurationProvider, RandomPortConfigProvider>();
            Container.RegisterSingleton<ILogFactory, DefaultLogFactory>();
            Container.RegisterSingleton<ISubProtocol<WebSocketSession>, CommandProtocol>();
            Container.RegisterSingleton<ISocketFactory, PassthroughSocketFactory>();
            Container.RegisterSingleton<IRequestInfoParser<SubRequestInfo>, JsonRequestInfoParser>();

            IEnumerable<Assembly> commands = new List<Assembly>();
            Container.RegisterCollection<ISubCommand<WebSocketSession>>(commands);

            Container.RegisterSingleton<ISerializer>(() =>
                new JsonNetSerializer(JsonSerializer.Create(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    ContractResolver = new NonPublicMembersContractResolver
                    {
                        CamelCase = true
                    }
                })));

            Container.RegisterSingleton<IServiceProvider>(() => Container);

            Container.Verify();
        }
    }
}
