using System;
using System.Collections.Generic;
using System.Reflection;
using AMQPOverWebSocketProxy.IOC;
using AMQPOverWebSocketProxy.Logging;
using AMQPOverWebSocketProxy.Serialization;
using AMQPOverWebSocketProxy.WebSocket;
using Common.Serialization;
using Common.Serialization.Serializer;
using Newtonsoft.Json;
using SimpleInjector;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Sockets;
using SuperSocket.WebSocket.SubProtocol;
using Topshelf;
using Topshelf.SimpleInjector;

namespace AMQPOverWebSocketProxy
{
    internal class Program
    {
        private static readonly Container Container = new Container();

        private static void Main()
        {
            Configure();

            HostFactory.Run(config =>
            {
                config.UseSimpleInjector(Container);
                config.UseNLog();

                config.Service<IService>(configurator =>
                {
                    configurator.ConstructUsingSimpleInjector();
                    configurator.WhenStarted((service, control) => service.Start(control, new SimpleInjectorDependencyResolverFactory(Container)));
                    configurator.WhenStopped(service => service.Stop());
                });

                config.SetDescription("A proxy for the AMQP protocol over WebSocket");
                config.SetDisplayName("AMQP over WebSocket Proxy");
                config.SetServiceName("AMQP over WebSocket Proxy");
            });
        }

        private static void Configure()
        {
            /* WebSocket */
            Container.RegisterSingleton<IBootstrap, SuperSocketBootStrapper>();
            Container.RegisterSingleton<ISuperSocketConfigurationProvider, SuperSocketAppConfigProvider>();
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

            Container.RegisterSingleton<IService, Service>();

            Container.RegisterSingleton<IServiceProvider>(() => Container);
            Container.Verify();
        }
    }
}
