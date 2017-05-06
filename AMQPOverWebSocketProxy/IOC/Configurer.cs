using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using AMQPOverWebSocketProxy.Actors;
using AMQPOverWebSocketProxy.Akka;
using AMQPOverWebSocketProxy.Logging;
using AMQPOverWebSocketProxy.Serialization;
using AMQPOverWebSocketProxy.WebSocket;
using AMQPOverWebSocketProxy.WebSocket.Commands;
using Common.Serialization;
using Common.Serialization.Serializer;
using Newtonsoft.Json;
using SimpleInjector;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Sockets;
using SuperSocket.WebSocket.SubProtocol;

namespace AMQPOverWebSocketProxy.IOC
{
    public class Configurer
    {
        public static void Configure(Container container, ActorSystem actorSystem)
        {
            /* WebSocket */
            container.RegisterSingleton<IBootstrap, SuperSocketBootStrapper>();
            container.RegisterSingleton<ISuperSocketConfigurationProvider, RandomPortConfigProvider>();
            container.RegisterSingleton<ILogFactory, DefaultLogFactory>();
            container.RegisterSingleton<ISubProtocol<WebSocketSession>, CommandProtocol>();
            container.RegisterSingleton<ISocketFactory, PassthroughSocketFactory>();
            container.RegisterSingleton<IRequestInfoParser<SubRequestInfo>, JsonRequestInfoParser>();

            container.RegisterCollection<ISubCommand<WebSocketSession>>(new[]
            {
                typeof(SendAmqpCommand)
            });

            var connectionPoolActorRegistration =
                Lifestyle.Singleton.CreateRegistration(
                    () => actorSystem.ActorOf(Props.Create(() => new ConnectionPoolActor())), container);
            container.RegisterConditional(typeof(IActorRef), connectionPoolActorRegistration, context => context.Consumer.ImplementationType == typeof(AmqpReceiveCommandActor));

            container.RegisterSingleton<IActorResolver>(() => new ActorResolver()
                .Register<UntypedActor<SubRequestActor<AmqpRequest<object>>.SubRequestParsed>, AmqpReceiveCommandActor>());

            container.RegisterSingleton<ISerializer>(() =>
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

            container.RegisterSingleton<IActorRefFactory>(() => actorSystem);
            container.RegisterSingleton<IDependencyResolver>(() => new SimpleInjectorDependencyResolver(container, actorSystem));
            container.RegisterSingleton<IServiceProvider>(() => container);

            container.Verify();
        }
    }
}