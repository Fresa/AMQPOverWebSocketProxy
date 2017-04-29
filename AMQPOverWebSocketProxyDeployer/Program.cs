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

            using (var system = ActorSystem.Create("Deployer", ConfigurationFactory.ParseString(@"
            akka {  
                actor{
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    deployment {
                        /socket-actor {
                            remote = ""akka.tcp://AMQP-over-WebSocket-Proxy-Actors@localhost:8090""
                        }
                    }
                }
                remote {
                    helios.tcp {
                        port = 0
                        hostname = localhost
                    }
                }
            }")))
            {
                var a = new SimpleInjectorDependencyResolverFactory(Container);
                a.Create(system);

                //var actor = system.ActorOf(Props.Create(() => new TestActor()), "socket-actor");
                //system.ActorOf(Props.Create(() => new SendActor(actor)), "sender");
                system.ActorOf(Props.Create(() => new SocketSupervisor2()), "socket-actor");

                Console.ReadKey();
            }
        }

        private static void DummyFunc()
        {

        }

        private static void Configure()
        {
            /* WebSocket */
            Container.Register<IBootstrap, SuperSocketBootStrapper>();
            Container.RegisterSingleton<ISuperSocketConfigurationProvider, RandomPortConfigProvider>();
            Container.RegisterSingleton<ILogFactory, DefaultLogFactory>();
            Container.Register<ISubProtocol<WebSocketSession>, CommandProtocol>();
            Container.Register<ISocketFactory, PassthroughSocketFactory>();
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

            //Container.RegisterSingleton<IService, Service>();

            Container.RegisterSingleton<IServiceProvider>(() => Container);

            Registration registration = Container.GetRegistration(typeof(IBootstrap)).Registration;
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                "Reason of suppression");
            Container.Verify();
        }
    }
}
